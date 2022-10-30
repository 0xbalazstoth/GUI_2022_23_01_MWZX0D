using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
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

            var jsonSerializer = new JsonSerializer();
            foreach (string save in saves)
            {
                string saveName = save.Split("\\")[1].Split('.')[0];
                
                lbSaves.Items.Add(saveName);
                //var data = File.ReadAllText($"Saves/{saveName}.json");
            }
            //// Get MainMenuWindow and close it
            //MainMenuWindow mainMenuWindow = (MainMenuWindow)Application.Current.MainWindow;
            //mainMenuWindow.Close();
            //mainMenuWindow.sound.Stop();
        }

        internal class Gamer
        {
            public string Name { get; set; }
            public Gamer(string Name)
            {
                this.Name = Name;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
