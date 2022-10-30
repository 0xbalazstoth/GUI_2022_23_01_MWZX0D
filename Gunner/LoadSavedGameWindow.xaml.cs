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
            //// Get MainMenuWindow and close it
            //MainMenuWindow mainMenuWindow = (MainMenuWindow)Application.Current.MainWindow;
            //mainMenuWindow.Close();
            //mainMenuWindow.sound.Stop();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
