using Logic.Game;
using Model.Game;
using Model.Game.Classes;
using Model.UI;
using SFML.Graphics;
using SFML.System;
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
        private Texture pistolBulletTexture;
        private Texture coinTexture;
        private Texture healthPotionTexture;
        private Dictionary<MovementDirection, Texture> playerTextures;

        public GameRenderer(IGameModel gameModel, string path)
        {
            this.gameModel = gameModel;
            this.assetsPath = path;

            // Note: Shouldn't be creating texture every frame, this is the reason of fps drops!
            pistolBulletTexture = new Texture("Assets/Textures/bullet_sheet.png");
            
            coinTexture = new Texture("Assets/Textures/coin_sheet.png");
            healthPotionTexture = new Texture("Assets/Textures/health_potion_sheet.png");
            
            playerTextures = new Dictionary<MovementDirection, Texture>();
            playerTextures.Add(MovementDirection.IdleRight, new Texture("Assets/Textures/idle_right.png"));
            playerTextures.Add(MovementDirection.IdleLeft, new Texture("Assets/Textures/idle_left.png"));
            playerTextures.Add(MovementDirection.Idle, new Texture("Assets/Textures/idle_right.png"));
            playerTextures.Add(MovementDirection.Left, new Texture("Assets/Textures/move_left.png"));
            playerTextures.Add(MovementDirection.Right, new Texture("Assets/Textures/move_right.png"));
            playerTextures.Add(MovementDirection.Up, new Texture("Assets/Textures/move_up.png"));
            playerTextures.Add(MovementDirection.Down, new Texture("Assets/Textures/move_down.png"));
        }

        public void Draw(RenderTarget window)
        {
            DrawTilemap(window);
            DrawCollectibleItems(window);
            DrawPlayer(window);
            DrawEnemy(window);
            DrawObjects(window);
            DrawBullets(window);
        }

        private void DrawBullets(RenderTarget window)
        {
            foreach (var pistolBullet in gameModel.Player.Gun.Bullets)
            {
                pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].Texture = pistolBulletTexture;
                pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].Sprite = new Sprite(pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].Texture);
                pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].TextureRect = new IntRect(0, 0, pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].GetSpriteSize.X, pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].GetSpriteSize.Y);
            }

            foreach (var bullet in gameModel.Player.Gun.Bullets)
            {
                window.Draw(bullet.Bullet);
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
                coin.Animations[coin.ItemType].Texture = coinTexture;
                coin.Animations[coin.ItemType].Sprite = new Sprite(coin.Animations[coin.ItemType].Texture);
                coin.Animations[coin.ItemType].Sprite.TextureRect = new IntRect(0, 0, coin.Animations[coin.ItemType].GetSpriteSize.X, coin.Animations[coin.ItemType].GetSpriteSize.Y);
            }

            foreach (CollectibleItemModel healthPotion in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Health_Potion))
            {
                healthPotion.Animations[healthPotion.ItemType].Texture = healthPotionTexture;
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
            gameModel.Player.Animations[MovementDirection.Idle].Texture = playerTextures[MovementDirection.Idle];
            gameModel.Player.Animations[MovementDirection.Idle].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Idle].Texture);
            gameModel.Player.Animations[MovementDirection.Idle].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Idle].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Idle].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Left].Texture = playerTextures[MovementDirection.Left];
            gameModel.Player.Animations[MovementDirection.Left].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Left].Texture);
            gameModel.Player.Animations[MovementDirection.Left].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Left].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Left].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Right].Texture = playerTextures[MovementDirection.Right];
            gameModel.Player.Animations[MovementDirection.Right].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Right].Texture);
            gameModel.Player.Animations[MovementDirection.Right].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Right].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Right].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Up].Texture = playerTextures[MovementDirection.Up];
            gameModel.Player.Animations[MovementDirection.Up].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Up].Texture);
            gameModel.Player.Animations[MovementDirection.Up].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Up].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Up].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.Down].Texture = playerTextures[MovementDirection.Down];
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
