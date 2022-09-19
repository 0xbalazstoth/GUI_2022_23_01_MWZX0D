using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace Model.Game.Classes
{
    public class GameModel : IGameModel
    {
        public View CameraView { get; set; }
        public View UIView { get; set; }
        public Movement Movement { get; set; }
        public TilemapModel Map { get; set; }
        public PlayerModel Player { get; set; }
        public EnemyModel Enemy { get; set; }
        public List<ChestModel> Chests { get ; set ; }
        public Dictionary<MovementDirection, Movement> MovementDirections { get; set; }
        public Vector2f MousePositionWindow { get; set; }
        public Vector2f WorldPositionInCamera { get; set; }
        public List<BulletModel> Bullets { get; set; }
    }
}
