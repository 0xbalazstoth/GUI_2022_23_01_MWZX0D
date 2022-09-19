using SFML.Graphics;
<<<<<<< HEAD
using SFML.System;
=======
>>>>>>> 4a6ff04bd44cea7113df8ad930cabf25a16cdb90
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
<<<<<<< HEAD
        public Vector2f MousePositionWindow { get; set; }
        public Vector2f WorldPositionInCamera { get; set; }
        public List<BulletModel> Bullets { get; set; }
=======
>>>>>>> 4a6ff04bd44cea7113df8ad930cabf25a16cdb90
    }
}
