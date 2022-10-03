using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class CollectibleObjectModel
    {
        public Guid Id { get; set; }
        public CircleShape Item { get; set; }
        public bool IsItemCollected { get; set; }
    }
}
