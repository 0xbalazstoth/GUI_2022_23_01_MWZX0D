using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public interface IGameModel
    {
        public Movement Movement { get; set; }
        public View CameraView { get; set; }
        public View UIView { get; set; }
        public TilemapModel Map { get; set; }
        public PlayerModel Player { get; set; }
        public EnemyModel Enemy { get; set; }
        public List<ChestModel> Chests { get; set; }
        public Dictionary<MovementDirection, Movement> MovementDirections { get; set; }
        public Vector2f MousePositionWindow { get; set; }
        public Vector2f WorldPositionInCamera { get; set; }
        public List<BulletModel> Bullets { get; set; }
        public List<GunModel> Guns { get; set; }
    }
}
