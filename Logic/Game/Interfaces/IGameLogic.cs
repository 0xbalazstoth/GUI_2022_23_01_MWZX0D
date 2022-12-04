using Model.Game.Classes;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Interfaces
{
    public interface IGameLogic
    {
        void UpdatePlayer(RenderWindow window);
        void UpdateEnemies(RenderWindow window);
        void UpdateDeltaTime();
        void UpdateCamera(View cameraView);
        void UpdateBullets(RenderWindow window);
        void UpdateTilemap();
        void MoveCamera(uint mapWidth, float dt);
        void SetTilemap(string tmxFile, string tilesetFile);
        Clock GetDeltaTimeClock { get; }
        float GetDeltaTime { get; }
        void CreateItems();
        void SpawnItems();
        void Music();
        void CameraEdges();
        List<Vector2f> GetSafeSpawnPoints();
        void CreateMaps();
        void PlayGameOverSound();
        void PlayGameWonSound();
    }
}
