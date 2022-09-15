using Logic.Game.Entities;
using Logic.Game.Interfaces;
using Model;
using Model.Game;
using Model.Game.Classes;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Classes
{
    public class GameLogic : IGameLogic 
    {
        // Minden logic használhat más logicot interfacen keresztül!

        private IGameModel gameModel;
        private ITilemapLogic tilemapLogic;
        private PlayerLogic playerLogic; // INTERFÉSZ
        private Clock deltaTimeClock;
        private float deltaTime;

        public Clock GetDeltaTimeClock { get => deltaTimeClock; }
        public float GetDeltaTime { get => deltaTime; }

        public GameLogic(IGameModel gameModel, ITilemapLogic tilemapLogic, PlayerLogic playerLogic)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;
            this.playerLogic = playerLogic;
            deltaTimeClock = new Clock();

            gameModel.CameraView = new View();
            gameModel.UIView = new View();

            gameModel.Map = new TilemapModel();
        }

        public void SetTilemap(string tmxFile, string tilesetFile)
        {
            TilemapLoader tmapLoader = new TilemapLoader(tilesetFile);
            tmapLoader.LoadTMXFile(tmxFile);
            tmapLoader.InitializeVertices();

            gameModel.Map.CollidableIDs = new List<int>();
            gameModel.Map.CollidableIDs.Add(4);
            gameModel.Map.TilesetTexture = new Texture(tilesetFile);
            gameModel.Map.Vertices = tmapLoader.Vertices;
            gameModel.Map.MapLayers = tmapLoader.MapLayers;
            gameModel.Map.Width = tmapLoader.Width;
            gameModel.Map.Height = tmapLoader.Height;
            gameModel.Map.TileWidth = tmapLoader.TileWidth;
            gameModel.Map.TileHeight = tmapLoader.TileHeight;
            gameModel.Map.Size = new Vector2u(tmapLoader.Width, tmapLoader.Height);
            gameModel.Map.TileSize = new Vector2u(tmapLoader.TileWidth, tmapLoader.TileHeight);
        }

        public void UpdatePlayer()
        {
            
        }

        public void UpdateDeltaTime()
        {
            deltaTime = deltaTimeClock.ElapsedTime.AsSeconds();
            deltaTimeClock.Restart();
        }

        public void UpdateCamera(View cameraView)
        {
            gameModel.CameraView = cameraView;
        }

        public void MoveCamera(uint mapWidth, Vector2f playerPosition, Vector2f cursorPositionWorld, float dt)
        {
            var direction = Vector2.Normalize(new(cursorPositionWorld.X - playerPosition.X, cursorPositionWorld.Y - playerPosition.Y));
            var position = new Vector2(playerPosition.X, playerPosition.Y);
            var distance = Vector2.Distance(new(playerPosition.X, playerPosition.Y), new(cursorPositionWorld.X, cursorPositionWorld.Y));

            position += direction * Math.Min(distance / 3f, 100f);

            var moveDirection = Vector2.Normalize(new(position.X - gameModel.CameraView.Center.X, position.Y - gameModel.CameraView.Center.Y));
            var speed = 500f * dt;
            var result = new Vector2(gameModel.CameraView.Center.X, gameModel.CameraView.Center.Y);
            result += moveDirection * speed;

            if (Vector2.Distance(result, position) < speed * 2f)
            {
                result = position;
            }

            if (!float.IsNaN(result.X) || !float.IsNaN(result.Y))
            {
                gameModel.CameraView.Center = new Vector2f(result.X, result.Y);
            }
        }

        public void SetView(ref View view, Vector2f size, Vector2f? center = null, FloatRect? viewport = null)
        {
            view = new View();
            view.Size = size;

            if (center != null)
            {
                view.Center = center.Value;
            }

            if (viewport != null)
            {
                view.Viewport = viewport.Value;
            }
        }
    }
}
