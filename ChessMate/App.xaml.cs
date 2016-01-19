using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChessMate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Chess chess = new Chess(ExitGame);
        }

        private void ExitGame(object sender, EventArgs e)
        {
            this.Shutdown();
        }
    }
}
