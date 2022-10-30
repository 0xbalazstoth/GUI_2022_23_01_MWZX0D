using Gunner.Controller;
using Model.Game.Classes;
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
using System.Windows.Shapes;

namespace Gunner
{
    /// <summary>
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        GameController gameController;

        public MainMenuWindow()
        {
            InitializeComponent();
        }
        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGameWindow newGameWindow = new NewGameWindow();
            newGameWindow.ShowDialog();
        }

        private void btnLoadGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHighscore_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnQuitGame_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if F11 is pressed
            if (e.Key == Key.F11)
            {
                // Check if window is in fullscreen mode
                if (WindowState == WindowState.Maximized)
                {
                    // Set window to normal mode
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;
                }
                else
                {
                    // Set window to fullscreen mode
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                }
            }
        }
    }
}
