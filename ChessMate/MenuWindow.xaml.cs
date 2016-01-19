using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessMate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow(RoutedEventHandler newGame, RoutedEventHandler loadGame, RoutedEventHandler exitGame)
        {
            InitializeComponent();
            New.Click += newGame;
            Load.Click += loadGame;
            Exit.Click += exitGame;
        }
        
    }
}
