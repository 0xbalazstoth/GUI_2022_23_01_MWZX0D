using Model.Game.Enums;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace Model.Game.Classes
{
    public class EnemyModel : UnitEntityModel
    {
        public string Name { get; set; }
        public EnemyType EnemyType { get; set; } = EnemyType.Eye;
        public Dictionary<MovementDirection, AnimationModel> Animations { get; set; }
        public int RewardXP { get; set; }
        public bool IsShooting { get; set; }
        public float SightDistance { get; set; }
        public List<Vector2i> Path { get; set; }
    }
}
