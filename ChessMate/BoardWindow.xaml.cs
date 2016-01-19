using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChessMate
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class BoardWindow : Window
    {
        private Position from;

        public BoardWindow()
        {
            InitializeComponent();

            Label turn = new Label();
            turn.Name = "turn";
            turn.FontSize = 60;
            Grid.SetRow(turn, 0);
            Grid.SetColumn(turn, 0);


            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    Label l = new Label();
                    l.FontSize = 43;

                    l.Padding = new Thickness(8, 0, 0, 0);

                    l.Name = "board_" + x + y;
                    l.BorderThickness = new Thickness(1, 1, 0, 0);
                    l.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    Grid.SetRow(l, 2 + y);
                    Grid.SetColumn(l, x);

                    ChessBoard.Children.Add(l);
                }
            }
        }

        public void repaint()
        {
            Dispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                String turn = Chess.WhosTurn();
                Turn.Content = char.ToUpper(turn[0]) + turn.Substring(1) + "'s turn";
                bool black = false;
                for (int y = 0; y < 8; ++y)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        Label l = FindChild<Label>(ChessBoard, "board_" + x + y);
                        if (l != null)
                        {
                            l.MouseLeftButtonDown -= showMoves;
                            l.MouseLeftButtonDown -= move;
                            l.MouseLeftButtonDown -= repaint;


                            Piece current = Chess.board.at(new Position(x, y));
                            String type = current.visual();
                            l.Content = type;
                            l.Background = black ? new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)) : new SolidColorBrush(Color.FromArgb(205, 255, 255, 255));

                            if (current.getColor() == Chess.WhosTurn())
                            {
                                if (current.getType() == "king")
                                {
                                    if (Chess.IsChecked(Chess.WhosTurn()))
                                    {
                                        l.Background = Brushes.LightCoral;
                                    }
                                }

                                l.Cursor = Cursors.Hand;
                                l.MouseLeftButtonDown += showMoves;
                            }
                            else
                            {
                                l.Cursor = Cursors.Arrow;
                            }
                        }
                        black = !black;
                    }
                    black = !black;
                }

                ChessMove latestMove = Chess.computerMove;
                if (latestMove != null)
                {
                    Label last = FindChild<Label>(ChessBoard, "board_" + latestMove.from.x + latestMove.from.y);
                    last.Background = Brushes.GreenYellow;

                    last = FindChild<Label>(ChessBoard, "board_" + latestMove.to.x + latestMove.to.y);
                    last.Background = Brushes.GreenYellow;
                }

            }));
        }

        private void showMoves(object sender, MouseButtonEventArgs e)
        {
            Label from = (Label)sender;
            from.Background = Brushes.LightBlue;

            int posX = Grid.GetColumn(from);
            int posY = Grid.GetRow(from) - 2;

            this.from = new Position(posX, posY);

            Dictionary<Position, bool?> moves = Chess.validMoves(Chess.board.at(this.from));

            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 8; ++y)
                {
                    Label l = FindChild<Label>(ChessBoard, "board_" + x + y);

                    l.MouseLeftButtonDown -= showMoves;

                    bool? validMove = moves[new Position(x, y)];
                    if (validMove != false)
                    {
                        l.Background = validMove == null ? Brushes.LightCoral : Brushes.LightGreen;
                        l.Cursor = Cursors.Hand;
                        l.MouseLeftButtonDown += move;
                    }
                    else
                    {
                        l.Cursor = Cursors.Arrow;
                        l.MouseLeftButtonDown += repaint;
                    }
                }
            }
        }

        private void repaint(object sender, MouseButtonEventArgs e)
        {
            repaint();
        }
        private void move(object sender, MouseButtonEventArgs e)
        {
            Label l = (Label)sender;
            Position to = new Position(Grid.GetColumn(l), Grid.GetRow(l) - 2);

            Chess.move(from, to);
            Chess.nextPlayer();
            Chess.computerMove = null;
            repaint();
        }
        public static T FindChild<T>(DependencyObject parent, string childName)
where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }


        public void updateProgress(int progress)
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (progress == -1)
                    {
                        Message.Content = "";
                    }
                    else
                    {
                        Message.Content = "Computer is thinking (" + progress + "%)";
                    }
                }));
            }
            catch
            {
                //Someone closed gamewindow, no need to do anything here.
            }
        }
    }
}
