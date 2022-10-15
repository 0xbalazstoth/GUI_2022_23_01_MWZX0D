using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using SFML.Audio;

namespace Model.Game.Classes
{
    public class GameModel : IGameModel
    {
        public View CameraView { get; set; }
        public View UIView { get; set; }
        public Movement Movement { get; set; }
        public TilemapModel Map { get; set; }
        public PlayerModel Player { get; set; }
        public List<EnemyModel> Enemies { get; set; }
        public List<IObjectEntity> Objects { get ; set ; }
        public Dictionary<MovementDirection, Movement> MovementDirections { get; set; }
        public Vector2f MousePositionWindow { get; set; }
        public Vector2f WorldPositionInCamera { get; set; }
        public List<GunModel> Guns { get; set; }
        public List<ICollectibleItem> CollectibleItems { get; set; }
    }
}
