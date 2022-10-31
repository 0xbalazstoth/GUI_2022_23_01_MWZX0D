using System.Collections.Generic;
using System.Windows;

namespace Gunner
{
    /// <summary>
    /// Interaction logic for LoadSavedGame.xaml
    /// </summary>
    public partial class LoadSavedGameWindow : Window
    {
        public LoadSavedGameWindow(string[] saves)
        {
            InitializeComponent();

            lbSaves.ItemsSource = saves;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
