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
    public class BulletModel : Sprite
    {
        public Vector2f Size { get; set; }
        public RectangleShape RectangleBullet { get; set; }
        public float Speed { get; set; }
    }
}
