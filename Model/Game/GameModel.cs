using SFML.Graphics;

namespace Model.Game
{
    public class GameModel : IGameModel
    {
        public View CameraView { get; set; }
        public View UIView { get; set; }
        public Movement Movement { get; set; }
        public Map Map { get; set; }
    }
}
