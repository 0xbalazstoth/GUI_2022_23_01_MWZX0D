using Model.Game.Interfaces;
using SFML.Audio;
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
        public List<EnemyModel> Enemies { get; set; }
        public List<IObjectEntity> Objects { get; set; }
        public Dictionary<MovementDirection, Movement> MovementDirections { get; set; }
        public Vector2f MousePositionWindow { get; set; }
        public Vector2f WorldPositionInCamera { get; set; }
        public List<GunModel> Guns { get; set; }
        public List<ICollectibleItem> CollectibleItems { get; set; }
        public List<Music> Musics { get; set; }
    }
}
