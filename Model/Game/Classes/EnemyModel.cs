using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public enum EnemyType
    {
        Enemy,
        Boss
    }

    public class EnemyModel : UnitEntityModel
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public EnemyType EnemyType { get; set; }
    }
}
