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
    }
}
