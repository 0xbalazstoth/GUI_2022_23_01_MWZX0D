using Repository.Classes;
using Repository.Exceptions;
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
    /// Interaction logic for NewGameWindow.xaml
    /// </summary>
    public partial class NewGameWindow : Window
    {

        public NewGameWindow()
        {
            InitializeComponent();
        }

        private void btnCreateNewGame_Click(object sender, RoutedEventArgs e)
        {
            // Create new game
            SaveHandler saveHandler = new SaveHandler();

            try
            {
                saveHandler.NewGame(txtBoxUsername.Text);

                // Run game
                //MainWindow mainWindow = new MainWindow(txtBoxUsername.Text);
                //mainWindow.Show();

                // Close this window
                Close();

                // Get MainMenuWindow and close it
                //MainMenuWindow mainMenuWindow = (MainMenuWindow)Application.Current.MainWindow;
                //mainMenuWindow.Hide();
                //mainMenuWindow.sound.Stop();
            }
            catch (SaveAlreadyExistsException error)
            {
                lblError.Visibility = Visibility.Visible;
                lblError.Text = error.Message;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
