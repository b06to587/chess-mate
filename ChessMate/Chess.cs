using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ChessMate
{
    class Chess
    {
        public static string gamePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ChessMate";
        public static string currentGameFile = "";

        public static string turn;
        public static MenuWindow menu;
        public static BoardWindow boardWindow;
        public static Board board;
        public static ChessMove lastMove;

        public Chess(RoutedEventHandler ExitGame)
        {
            menu = new MenuWindow(NewGame, LoadGame, ExitGame);
            ShowMenu();
        }

        private void ShowMenu()
        {
            menu.Show();
        }
        private void NewGame(object sender, RoutedEventArgs e)
        {
            newGame("INVALID_PATH");
        }


        Timer t;
        AutoResetEvent are;
        public void newGame(string loadedGamePath)
        {
            menu.Hide();

            gamePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ChessMate";
            lastMove = null;
            computerMove = null;
            turn = "white";
            board = new Board();
            boardWindow = new BoardWindow();
            boardWindow.Show();
            boardWindow.Closing += ExitGame;

            boardWindow.repaint();

            if (loadedGamePath.Equals("INVALID_PATH"))
            {
                DataHandler.writeToJSON();
            }

            are = new AutoResetEvent(true);
            t = new Timer(checkforchange, are, 1000, 5000);

        }

        private void checkforchange(object state)
        {
            DataHandler.checkForChanges();
        }

        private void LoadGame(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "ChessGame|*.json"; // Filter files by extension 

            if (!Directory.Exists(gamePath))
            {
                Directory.CreateDirectory(gamePath);
            }

            dlg.InitialDirectory = gamePath.ToString();

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                newGame(dlg.FileName);
                currentGameFile = dlg.FileName;
                loadGame();
            }
        }

        public static void loadGame()
        {
            DataHandler.loadFromJSON();
            lastMove = null;
            boardWindow.repaint();

            if (turn == "black")
            {
                turn = "white";
                nextPlayer();
            }
        }
        private void ExitGame(object sender, EventArgs e)
        {
            try
            {
                boardWindow.Close();
            }
            catch (Exception) { }
            ShowMenu();
        }

        public static string WhosTurn()
        {
            return turn;
        }

        public static Dictionary<Position, bool?> validMoves(Piece toBeMoved)
        {
            Dictionary<Position, bool?> moves = new Dictionary<Position, bool?>();
            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 8; ++y)
                {
                    Position to = new Position(x, y);
                    Piece moveTo = board.at(to);
                    bool? valid = Rules.validMove(toBeMoved, to);

                    if (valid == true && moveTo.getType() != "blank")
                    {
                        valid = null;
                    }
                    if (valid != false)
                    {
                        move(toBeMoved.getPosition(), moveTo.getPosition(), false);
                        if (IsChecked(toBeMoved.getColor()))
                        {
                            valid = false;
                        }

                        undo();
                    }
                    moves.Add(to, valid);
                }
            }

            return moves;
        }

        public static void undo()
        {
            Piece last = board.at(lastMove.to);
            if (last.getType() == "pawn" && (lastMove.from.y == 1 && last.getColor() == "black" || lastMove.from.y == 6 && last.getColor() == "white"))
            {
                last.numberOfMoves = 0;
            }
            else
            {
                last.numberOfMoves--;

            }
            last.setPosition(lastMove.from);
            board.set(lastMove.from, last);
            board.set(lastMove.to, pieceRemoved);
        }

        public static bool checkingOwn = false;
        public static bool IsSame(Piece p, Position pos)
        {
            return p.getColor() == board.at(pos).getColor() && !checkingOwn;
        }

        public static bool IsEmpty(Position pos)
        {
            Debug.WriteLine(pos.x + "," + pos.y);
            return board.at(pos).getType() == "blank";
        }

        public static bool hasMoves()
        {
            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 8; ++y)
                {
                    Piece p = board.at(new Position(x, y));
                    if (p.getColor() != turn && p.getType() != "blank")
                    {
                        Position pos = new Position(x, y);
                        Dictionary<Position, bool?> moves = Chess.validMoves(p);
                        foreach (KeyValuePair<Position, bool?> kvp in moves)
                        {
                            if (kvp.Value != false)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static void AIMove()
        {
            int progress = 0;
            Random rnd = new Random();
            while (progress <= 100)
            {
                boardWindow.updateProgress(progress);
                ++progress;
                Thread.Sleep((int)(rnd.NextDouble() * rnd.NextDouble() * 100));
            }
            boardWindow.updateProgress(-1);
            computerMove = AI.nextDraw();
            move(computerMove.from, computerMove.to);
            Application.Current.Dispatcher.Invoke(new Action(() => { nextPlayer(); }));
        }

        public static ChessMove computerMove;
        private static Thread AIThread;
        public static void nextPlayer()
        {
            Debug.WriteLine(board.at(new Position(0, 7)).numberOfMoves);
            bool hasMoves = Chess.hasMoves();

            if (hasMoves)
            {
                if (turn == "white")
                {
                    turn = "black";
                    DataHandler.writeToJSON();
                    boardWindow.repaint();
                    AIThread = new Thread(AIMove);
                    AIThread.Start();
                }
                else
                {
                    turn = "white";
                    DataHandler.writeToJSON();
                }
            }
            else
            {
                checkMate();
            }
            boardWindow.repaint();
        }

        private static void checkMate()
        {
            boardWindow.repaint();
            MessageBox.Show(turn + " won!");

            try
            {
                boardWindow.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        public static bool IsChecked(string color)
        {
            Position kingPos = board.findKing(color);
            Piece king = board.at(kingPos);

            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 8; ++y)
                {
                    Position from = new Position(x, y);
                    Piece p = board.at(from);
                    if (p.getColor() != king.getColor())
                    {
                        if (Rules.validMove(p, kingPos))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static Piece pieceRemoved;

        private static void move(Position from, Position to, bool realMove){
            lastMove = new ChessMove(from, to);
            Piece p1 = board.at(from);
            Piece p2 = board.at(to);
            pieceRemoved = p2;

            int xDiff = to.x - from.x;
            if (p1.getType() == "king" && Math.Abs(xDiff) > 1 && realMove)
            {
                int xDir = xDiff < 0 ? -2 : 1;
                Position rook = new Position(to.x + xDir, to.y);
                xDir = xDiff < 0 ? 1 : -1;
                board.at(rook).debug();
                move(rook, new Position(to.x + xDir, to.y));
            }

            board.set(from, new Piece("blank", "none", from));

            p1.numberOfMoves++;
            if (p1.getType() == "pawn" && Math.Abs(to.y - from.y) == 2)
            {
                p1.numberOfMoves++;
            }

            p1.setPosition(to);

            if (p1.getType() == "pawn" && (to.y == 0 || to.y == 7))
            {
                p1 = new Piece("queen", p1.getColor(), p1.getPosition(), p1.getNumberOfMoves());
            }

            board.set(to, p1);
        }

        public static void move(Position from, Position to)
        {
            if (!Rules.validMove(board.at(from), to))
            {
                throw new Exception("InvalidMoveError");
            }
            move(from, to, true);
        }
        
        public static void tempMove(Position from, Position to)
        {
            move(from, to, false);
        }

        private static List<Piece> getPieces(string color)
        {
            List<Piece> pieces = new List<Piece>();

            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 8; ++y)
                {
                    Position pos = new Position(x, y);
                    Piece p = board.at(pos);
                    if (p.getColor().Equals(color))
                    {
                        pieces.Add(p);
                    }
                }
            }
            return pieces;
        }

        public static List<Piece> blackPieces()
        {
            return getPieces("black");
        }
        public static List<Piece> whitePieces()
        {
            return getPieces("white");
        }

    }
}
