using Logic.Game.Interfaces;
using Model.Game.Classes;
using Model.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private IPlayerLogic playerLogic;
        private ObservableCollection<ICollectibleItem> items = new ObservableCollection<ICollectibleItem>();

        public InventoryWindow(IGameModel gameModel, IPlayerLogic playerLogic)
        {
            InitializeComponent();

            this.gameModel = gameModel;
            this.playerLogic = playerLogic;

            foreach (var item in gameModel.Player.Inventory.Items)
            {
                items.Add(item.Value);
            }

            lstBoxInventory.ItemsSource = items;
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

        private void lstBoxInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            if (item.SelectedItem != null)
            {
                // Selected item
                var selectedItem = item.SelectedItem;
                var selectedItemValue = (ICollectibleItem)selectedItem;

                // Update inventory
                playerLogic.UseItemFromInventory(selectedItemValue);

                // If item's quantity is 0, then remove from list
                if (selectedItemValue.Quantity == 0)
                {
                    items.Remove(selectedItemValue);
                }

                // Refresh inventory list box
                lstBoxInventory.Items.Refresh();
            }

            // Needed because of reselect
            lstBoxInventory.UnselectAll();
        }
    }
}
