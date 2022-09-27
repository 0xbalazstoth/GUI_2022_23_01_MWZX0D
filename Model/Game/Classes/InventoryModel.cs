using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class InventoryModel
    {
        public int MaxItemsCount { get; set; }
        public List<Drawable> Items { get; set; }
    }
}
