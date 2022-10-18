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
        private List<ICollectibleItem> items;

        public List<ICollectibleItem> Items { get; }

        public InventoryWindow(IGameModel gameModel)
        {
            InitializeComponent();

            this.gameModel = gameModel;

            lstBoxInventory.ItemsSource = gameModel.Player.Inventory.Items;
        }
    }
}
