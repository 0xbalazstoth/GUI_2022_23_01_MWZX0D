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
        void UpdatePlayer();
        void UpdateDeltaTime();
        void UpdateCamera(View cameraView);
        void MoveCamera(uint mapWidth, Vector2f playerPosition, Vector2f cursorPositionWorld, float dt);
        void SetView(ref View cameraView, Vector2f size, Vector2f? center = null, FloatRect? viewport = null);
        void SetTilemap(string tmxFile, string tilesetFile);
        Clock GetDeltaTimeClock { get; }
        float GetDeltaTime { get; }
    }
}
