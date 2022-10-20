using Model.Game.Classes;
using Model.Game.Interfaces;
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
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class InventoryWindow : Window
    {
        private IGameModel gameModel;

        public List<ICollectibleItem> Items { get; }

        public InventoryWindow(IGameModel gameModel)
        {
            InitializeComponent();

            this.gameModel = gameModel;

            lstBoxInventory.ItemsSource = gameModel.Player.Inventory.Items;
            gameModel.Player.IsFocusedInGame = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gameModel.Player.IsFocusedInGame = true;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            // Move this window by MainWindow's position and center it
            this.Top = Application.Current.MainWindow.Top + (Application.Current.MainWindow.Height / 2) - (this.Height / 2);
            this.Left = Application.Current.MainWindow.Left + (Application.Current.MainWindow.Width / 2) - (this.Width / 2);
        }
    }
}
