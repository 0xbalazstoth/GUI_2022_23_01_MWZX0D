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
    /// Interaction logic for PauseMenu.xaml
    /// </summary>
    public partial class PauseMenu : Window
    {
        public PauseMenu()
        {
            InitializeComponent();
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSaveGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExitToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow mainMenuWindow = (MainMenuWindow)Application.Current.MainWindow;
            mainMenuWindow.Show();

            MainWindow gameWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            gameWindow.Hide();
            gameWindow.Close();
            gameWindow.Content = null;
            gameWindow.Focusable = false;
            gameWindow.IsEnabled = false;

            Close();
        }

        private void btnQuitGame_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
