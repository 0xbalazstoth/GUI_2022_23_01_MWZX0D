using Logic.Game;
using Model.Game;
using Model.Game.Classes;
using Model.UI;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
    public class GameRenderer
    {
        private IGameModel gameModel;
        private string assetsPath;

        public GameRenderer(IGameModel gameModel, string path)
        {
            this.gameModel = gameModel;
            this.assetsPath = path;
        }

        public void Draw(RenderTarget window)
        {
            DrawTilemap(window);
            DrawPlayer(window);
            DrawEnemy(window);
            DrawObjects(window);
        }

        private void DrawObjects(RenderTarget window)
        {
            foreach (var chest in gameModel.Chests)
            {
                window.Draw(chest);
            }
        }

        private void DrawEnemy(RenderTarget window)
        {
            window.Draw(gameModel.Enemy);
        }

        private void DrawPlayer(RenderTarget window)
        {
            window.Draw(gameModel.Player);
        }

        private void DrawTilemap(RenderTarget window)
        {
            for (int i = 0; i < gameModel.Map.Vertices.Count; i++)
            {
                window.Draw(gameModel.Map.Vertices[i], PrimitiveType.Quads, new(BlendMode.Alpha, Transform.Identity, gameModel.Map.TilesetTexture, null));
            }
        }
    }
}
