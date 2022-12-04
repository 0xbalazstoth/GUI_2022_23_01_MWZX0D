using Model.Game.Enums;
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
        public TilemapModel CurrentMap { get; set; }
        public TilemapModel KillArenaMap { get; set; }
        public List<int> MapCollidibleIDs { get; }
        public TilemapModel LobbyMap { get; set; }
        public TilemapModel BossMap { get; set; }
        public PlayerModel Player { get; set; }
        public List<EnemyModel> Enemies { get; set; }
        public List<IObjectEntity> Objects { get; set; }
        public Dictionary<MovementDirection, Movement> MovementDirections { get; set; }
        public Vector2f MousePositionWindow { get; set; }
        public Vector2f WorldPositionInCamera { get; set; }
        public List<GunModel> Guns { get; set; }
        public List<ICollectibleItem> CollectibleItems { get; set; }
        public List<Music> Musics { get; set; }
        public bool DebugMode { get; set; }
        public List<GateModel> Gates { get; set; }
        public List<Text> CreatorTexts { get; set; }
        public List<Text> SettingsTexts { get; set; }
        public SoundBuffer TeleportSoundBuffer { get; set; }
        public Sound TeleportSound { get; set; }
        public SoundBuffer GameOverSoundBuffer { get; set; }
        public Sound GameOverSound { get; set; }
        public SoundBuffer SpeedPotionSoundBuffer { get; set; }
        public Sound SpeedPotionSound { get; set; }
        public SoundBuffer HealthPotionSoundBuffer { get; set; }
        public Sound HealthPotionSound { get; set; }
        public SoundBuffer GameWonSoundBuffer { get; set; }
        public Sound GameWonSound { get; set; }
    }
}
