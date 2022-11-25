using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using SFML.Audio;
using Model.Game.Enums;

namespace Model.Game.Classes
{
    public class GameModel : IGameModel
    {
        public View CameraView { get; set; }
        public View UIView { get; set; }
        public Movement Movement { get; set; }
        public TilemapModel CurrentMap { get; set; }
        public TilemapModel KillArenaMap { get; set; }
        public TilemapModel LobbyMap { get; set; }
        public TilemapModel BossMap { get; set; }
        public List<int> MapCollidibleIDs
        {
            get
            {
                return new List<int>() { 9, 10, 13, 98, 1, 2, 147, 198, 50, 49, 151, 102, 53, 52, 3 };
            }
        }
        public PlayerModel Player { get; set; }
        public List<EnemyModel> Enemies { get; set; }
        public List<IObjectEntity> Objects { get ; set ; }
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
    }
}
