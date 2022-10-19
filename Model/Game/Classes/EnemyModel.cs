using Model.Game.Enums;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace Model.Game.Classes
{
    public class EnemyModel : UnitEntityModel
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public EnemyType EnemyType { get; set; } = EnemyType.Basic;
        public Dictionary<EnemyType, AnimationModel> Animations { get; set; }
    }
}
