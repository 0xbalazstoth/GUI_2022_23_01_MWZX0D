using SFML.Graphics;
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
    }
}
