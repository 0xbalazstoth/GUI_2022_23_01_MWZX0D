using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class ChestModel : Sprite, IObjectEntity
    {
        public int MaxItemsCount { get; set; }
        public Vector2i Size { get; set; }
    }
}
