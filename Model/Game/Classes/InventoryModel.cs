using Model.Game.Interfaces;
using System;
using System.Collections.Generic;

namespace Model.Game.Classes
{
    public class InventoryModel
    {
        public Dictionary<int,ICollectibleItem>  Items { get; set; }

        public Dictionary<int, int> Quantities { get; set; }

        public int Capacity { get; set; }

        public int MaxCapacity { get; set; } = 10;
        public int Quantity { get; set; }
    }
}
