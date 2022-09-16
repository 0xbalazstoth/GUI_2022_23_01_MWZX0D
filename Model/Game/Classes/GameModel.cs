using SFML.Graphics;

namespace Model.Game.Classes
{
    public class GameModel : IGameModel
    {
        public View CameraView { get; set; }
        public View UIView { get; set; }
        public Movement Movement { get; set; }
        public TilemapModel Map { get; set; }
        public UnitEntityModel UnitEntity { get; set; }
        public PlayerModel Player { get; set; }
    }
}
