using Logic.Game.Interfaces;
using Model.Game.Classes;
using Model.Game.Enums;
using Model.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ShopWindow.xaml
    /// </summary>
    public partial class ShopWindow : Window
    {
        private IGameModel gameModel;
        private IPlayerLogic playerLogic;
        private ObservableCollection<ICollectibleItem> items = new ObservableCollection<ICollectibleItem>();

        public ShopWindow(IGameModel gameModel, IPlayerLogic playerLogic)
        {
            InitializeComponent();

            this.gameModel = gameModel;
            this.playerLogic = playerLogic;

            items.Add(new CollectibleItemModel() { ItemType = ItemType.Health_Potion, Price = 5, Id = (int)ItemType.Health_Potion });
            items.Add(new CollectibleItemModel() { ItemType = ItemType.Speed_Potion, Price = 10, Id = (int)ItemType.Speed_Potion });

            lstBoxItems.ItemsSource = items;
            gameModel.Player.IsFocusedInGame = false;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            // Move this window by MainWindow's position and center it
            this.Top = Application.Current.MainWindow.Top + (Application.Current.MainWindow.Height / 2) - (this.Height / 2);
            this.Left = Application.Current.MainWindow.Left + (Application.Current.MainWindow.Width / 2) - (this.Width / 2);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gameModel.Player.IsFocusedInGame = true;
            gameModel.Player.PlayerState = GateState.InLobby;
        }

        private void lstBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            if (item.SelectedItem != null)
            {
                // Selected item
                var selectedItem = item.SelectedItem;
                var selectedItemValue = (ICollectibleItem)selectedItem;

                playerLogic.BuyItemFromShop(selectedItemValue);

                // Refresh inventory list box
                lstBoxItems.Items.Refresh();
            }

            // Needed because of reselect
            lstBoxItems.UnselectAll();
        }
    }
}
