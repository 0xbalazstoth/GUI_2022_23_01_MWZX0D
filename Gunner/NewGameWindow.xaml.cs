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
        private string playerName;

        public string PlayerName
        {
            get { return playerName; }
        }

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
                this.playerName = txtBoxUsername.Text;
                this.DialogResult = true;

                Close();
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
