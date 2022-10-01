using Model.Game.Enums;

namespace Model.Game.Classes
{
    public class EnemyModel : UnitEntityModel
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public EnemyType EnemyType { get; set; }
    }
}
