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

        public GameRenderer(IGameModel gameModel, string assetsPath)
        {
            this.gameModel = gameModel;
            this.assetsPath = assetsPath;
        }

        public void Draw(RenderTarget window)
        {
            //window.Draw(DrawablePlayer());
            DrawableTilemap(window);
        }

        //private Drawable DrawablePlayer()
        //{
        //    gameModel.Player.Texture = new Texture(Path.Combine(assetsPath, "player.png"));
        //    return gameModel.Player;
        //}

        private void DrawableTilemap(RenderTarget window)
        {
            for (int i = 0; i < gameModel.Map.Vertices.Count; i++)
            {
                window.Draw(gameModel.Map.Vertices[i], PrimitiveType.Quads, new(BlendMode.Alpha, Transform.Identity, gameModel.Map.TilesetTexture, null));
            }
        }
    }
}
