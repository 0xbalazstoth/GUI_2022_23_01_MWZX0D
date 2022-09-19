using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class PlayerModel : UnitEntityModel
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public List<BulletModel> Bullets { get; set; }
        public Vector2f Center { get; set; }
        public Vector2f AimDirection { get; set; }
        public Vector2f AimDirectionNormalized { get; set; }
    }
}
