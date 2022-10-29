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
            MainMenuWindow mainMenuWindow = new MainMenuWindow();
            //Save();
            mainMenuWindow.ShowDialog();
            Close();
            
        }

        private void btnQuitGame_Click(object sender, RoutedEventArgs e)
        {
            //Save();
            Environment.Exit(0);
        }
    }
}
