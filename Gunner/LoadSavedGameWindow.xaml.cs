using System.Collections.Generic;
using System.Windows;

namespace Gunner
{
    /// <summary>
    /// Interaction logic for LoadSavedGame.xaml
    /// </summary>
    public partial class LoadSavedGameWindow : Window
    {
        private string selectedSave;

        public string SelectedSave
        {
            get { return selectedSave; }
        }

        public LoadSavedGameWindow(string[] saves)
        {
            InitializeComponent();

            lbSaves.ItemsSource = saves;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void lbSaves_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Get selected value
            var selectedItem = lbSaves.SelectedItem.ToString();
            selectedSave = selectedItem;
            this.DialogResult = true;
        }
    }
}
