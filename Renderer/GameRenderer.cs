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
            DrawBullets(window);
            DrawCollectibleItems(window);
        }

        private void DrawBullets(RenderTarget window)
        {
            foreach (var bullet in gameModel.Player.Bullets)
            {
                window.Draw(bullet.Shape);
            }
        }

        private void DrawObjects(RenderTarget window)
        {
            foreach (ChestModel chest in gameModel.Objects)
            {
                window.Draw(chest);
            }
        }

        private void DrawEnemy(RenderTarget window)
        {
            window.Draw(gameModel.Enemy);
        }

        private void DrawCollectibleItems(RenderTarget window)
        {
            foreach (CollectibleItemModel coin in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Coin))
            {
                coin.Animations[coin.ItemType].Texture = new Texture("Assets/Textures/coin_sheet.png");
                coin.Animations[coin.ItemType].Sprite = new Sprite(coin.Animations[coin.ItemType].Texture);
                coin.Animations[coin.ItemType].Sprite.TextureRect = new IntRect(0, 0, coin.Animations[coin.ItemType].GetSpriteSize.X, coin.Animations[coin.ItemType].GetSpriteSize.Y);
            }

            foreach (CollectibleItemModel healthPotion in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Health_Potion))
            {
                healthPotion.Animations[healthPotion.ItemType].Texture = new Texture("Assets/Textures/health_potion_sheet.png");
                healthPotion.Animations[healthPotion.ItemType].Sprite = new Sprite(healthPotion.Animations[healthPotion.ItemType].Texture);
                healthPotion.Animations[healthPotion.ItemType].Sprite.TextureRect = new IntRect(0, 0, healthPotion.Animations[healthPotion.ItemType].GetSpriteSize.X, healthPotion.Animations[healthPotion.ItemType].GetSpriteSize.Y);
            }

            foreach (var item in gameModel.CollectibleItems)
            {
                window.Draw(item.Item);
            }
        }

        private void DrawPlayer(RenderTarget window)
        {
            gameModel.Player.Animations[MovementDirection.Idle].Texture = new Texture("idle_right.png");
            gameModel.Player.Animations[MovementDirection.Idle].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Idle].Texture);
            gameModel.Player.Animations[MovementDirection.Idle].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Idle].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Idle].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Left].Texture = new Texture("move_left.png");
            gameModel.Player.Animations[MovementDirection.Left].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Left].Texture);
            gameModel.Player.Animations[MovementDirection.Left].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Left].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Left].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Right].Texture = new Texture("move_right.png");
            gameModel.Player.Animations[MovementDirection.Right].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Right].Texture);
            gameModel.Player.Animations[MovementDirection.Right].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Right].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Right].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Up].Texture = new Texture("move_up.png");
            gameModel.Player.Animations[MovementDirection.Up].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Up].Texture);
            gameModel.Player.Animations[MovementDirection.Up].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Up].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Up].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Down].Texture = new Texture("move_down.png");
            gameModel.Player.Animations[MovementDirection.Down].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Down].Texture);
            gameModel.Player.Animations[MovementDirection.Down].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Down].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Down].GetSpriteSize.Y);

            window.Draw(gameModel.Player);
            window.Draw(gameModel.Player.Gun);
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
