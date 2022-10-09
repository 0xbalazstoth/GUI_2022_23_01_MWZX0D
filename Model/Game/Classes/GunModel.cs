using Model.Game.Enums;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class GunModel : Sprite
    {
        public int MaxAmmo { get; set; }
        public int CurrentAmmo { get; set; }
        public int Damage { get; set; }
        public GunType GunType { get; set; }
        public List<BulletModel> Bullets { get; set; }
    }
}
