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
        private Texture bulletSheetTexture;
        private Texture coinTexture;
        private Texture healthPotionTexture;
        private Texture speedPotionTexture;
        private Dictionary<MovementDirection, Texture> playerTextures;
        private Texture enemyTexture;
        private Texture pistolTexture;
        private Texture shotgunTexture;
        private Texture hpTexture;

        public GameRenderer(IGameModel gameModel, string path)
        {
            this.gameModel = gameModel;
            this.assetsPath = path;

            // Note: Shouldn't be creating texture every frame, this is the reason of fps drops!
            bulletSheetTexture = new Texture("Assets/Textures/bullet_sheet.png");
            
            coinTexture = new Texture("Assets/Textures/coin_sheet.png");
            healthPotionTexture = new Texture("Assets/Textures/health_potion_sheet.png");
            speedPotionTexture = new Texture("Assets/Textures/speed_potion_sheet.png");
            enemyTexture = new Texture("Assets/Textures/player.png");
            
            playerTextures = new Dictionary<MovementDirection, Texture>();
            playerTextures.Add(MovementDirection.IdleRight, new Texture("Assets/Textures/idle_right.png"));
            playerTextures.Add(MovementDirection.IdleLeft, new Texture("Assets/Textures/idle_left.png"));
            playerTextures.Add(MovementDirection.Idle, new Texture("Assets/Textures/idle_right.png"));
            playerTextures.Add(MovementDirection.Left, new Texture("Assets/Textures/move_left.png"));
            playerTextures.Add(MovementDirection.Right, new Texture("Assets/Textures/move_right.png"));
            playerTextures.Add(MovementDirection.Up, new Texture("Assets/Textures/move_up.png"));
            playerTextures.Add(MovementDirection.Down, new Texture("Assets/Textures/move_down.png"));

            pistolTexture = new Texture("Assets/Textures/pistol.png");
            shotgunTexture = new Texture("Assets/Textures/shotgun.png");

            hpTexture = new Texture("Assets/Textures/heart.png");
        }

        public void Draw(RenderTarget window)
        {
            DrawTilemap(window);
            DrawCollectibleItems(window);
            DrawEnemy(window);
            DrawPlayer(window);
            DrawObjects(window);
            DrawBullets(window);
        }

        private void DrawBullets(RenderTarget window)
        {
            if (gameModel.Player.Gun.GunType == Model.Game.Enums.GunType.Pistol)
            {
                foreach (var pistolBullet in gameModel.Player.Gun.Bullets)
                {
                    pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].Texture = bulletSheetTexture;
                    pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].Sprite = new Sprite(pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].Texture);
                    pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].TextureRect = new IntRect(0, 0, pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].GetSpriteSize.X, pistolBullet.Animations[Model.Game.Enums.GunType.Pistol].GetSpriteSize.Y);
                }
            }
            else if (gameModel.Player.Gun.GunType == Model.Game.Enums.GunType.Shotgun)
            {
                foreach (var shotgunBullet in gameModel.Player.Gun.Bullets)
                {
                    shotgunBullet.Animations[Model.Game.Enums.GunType.Shotgun].Texture = bulletSheetTexture;
                    shotgunBullet.Animations[Model.Game.Enums.GunType.Shotgun].Sprite = new Sprite(shotgunBullet.Animations[Model.Game.Enums.GunType.Shotgun].Texture);
                    shotgunBullet.Animations[Model.Game.Enums.GunType.Shotgun].TextureRect = new IntRect(0, 0, shotgunBullet.Animations[Model.Game.Enums.GunType.Shotgun].GetSpriteSize.X, shotgunBullet.Animations[Model.Game.Enums.GunType.Shotgun].GetSpriteSize.Y);
                }
            }

            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                if (gameModel.Enemies[i].Gun.GunType == Model.Game.Enums.GunType.Pistol)
                {
                    for (int j = 0; j < gameModel.Enemies[i].Gun.Bullets.Count; j++)
                    {
                        gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Pistol].Texture = bulletSheetTexture;
                        gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Pistol].Sprite = new Sprite(gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Pistol].Texture);
                        gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Pistol].TextureRect = new IntRect(0, 0, gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Pistol].GetSpriteSize.X, gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Pistol].GetSpriteSize.Y);
                    }
                }
                else if (gameModel.Enemies[i].Gun.GunType == Model.Game.Enums.GunType.Shotgun)
                {
                    for (int j = 0; j < gameModel.Enemies[i].Gun.Bullets.Count; j++)
                    {
                        gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Shotgun].Texture = bulletSheetTexture;
                        gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Shotgun].Sprite = new Sprite(gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Shotgun].Texture);
                        gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Shotgun].TextureRect = new IntRect(0, 0, gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Shotgun].GetSpriteSize.X, gameModel.Enemies[i].Gun.Bullets[j].Animations[Model.Game.Enums.GunType.Shotgun].GetSpriteSize.Y);
                    }
                }
            }

            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                for (int j = 0; j < gameModel.Enemies[i].Gun.Bullets.Count; j++)
                {
                    window.Draw(gameModel.Enemies[i].Gun.Bullets[j].Bullet);
                }
            }

            foreach (var bullet in gameModel.Player.Gun.Bullets)
            {
                window.Draw(bullet.Bullet);
            }

            
        }

        private void DrawObjects(RenderTarget window)
        {
            foreach (ObjectEntityModel chest in gameModel.Objects)
            {
                window.Draw(chest);
            }

            foreach (var pistol in gameModel.Guns.Where(x => x.GunType == Model.Game.Enums.GunType.Pistol))
            {
                pistol.Texture = pistolTexture;
                pistol.TextureRect = new IntRect(0, 0, 12, 3);
            }

            foreach (var shotgun in gameModel.Guns.Where(x => x.GunType == Model.Game.Enums.GunType.Shotgun))
            {
                shotgun.Texture = shotgunTexture;
                shotgun.TextureRect = new IntRect(0, 0, 16, 6);
            }
        }

        private void DrawEnemy(RenderTarget window)
        {
            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].Texture = enemyTexture;
                window.Draw(gameModel.Enemies[i]);

                if (gameModel.Enemies[i].Gun.GunType == Model.Game.Enums.GunType.Pistol)
                {
                    gameModel.Enemies[i].Gun.Texture = pistolTexture;
                    gameModel.Enemies[i].Gun.TextureRect = new IntRect(0, 0, 12, 3);
                }
                else if (gameModel.Enemies[i].Gun.GunType == Model.Game.Enums.GunType.Shotgun)
                {
                    gameModel.Enemies[i].Gun.Texture = shotgunTexture;
                    gameModel.Enemies[i].Gun.TextureRect = new IntRect(0, 0, 16, 6);
                }

                window.Draw(gameModel.Enemies[i].Gun);

                // Draw HP
                gameModel.Enemies[i].HPSprite.Texture = hpTexture;
                window.Draw(gameModel.Enemies[i].HPSprite);
                window.Draw(gameModel.Enemies[i].HPText);
            }
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

            foreach (CollectibleItemModel speedPotion in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Speed_Potion))
            {
                speedPotion.Animations[speedPotion.ItemType].Texture = speedPotionTexture;
                speedPotion.Animations[speedPotion.ItemType].Sprite = new Sprite(speedPotion.Animations[speedPotion.ItemType].Texture);
                speedPotion.Animations[speedPotion.ItemType].Sprite.TextureRect = new IntRect(0, 0, speedPotion.Animations[speedPotion.ItemType].GetSpriteSize.X, speedPotion.Animations[speedPotion.ItemType].GetSpriteSize.Y);
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

            gameModel.Player.Animations[MovementDirection.IdleLeft].Texture = playerTextures[MovementDirection.IdleLeft];
            gameModel.Player.Animations[MovementDirection.IdleLeft].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.IdleLeft].Texture);
            gameModel.Player.Animations[MovementDirection.IdleLeft].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.IdleLeft].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.IdleLeft].GetSpriteSize.Y);

            gameModel.Player.Animations[MovementDirection.IdleRight].Texture = playerTextures[MovementDirection.IdleRight];
            gameModel.Player.Animations[MovementDirection.IdleRight].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.IdleRight].Texture);
            gameModel.Player.Animations[MovementDirection.IdleRight].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.IdleRight].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.IdleRight].GetSpriteSize.Y);

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

            // Draw HP
            gameModel.Player.HPSprite.Texture = hpTexture;
            window.Draw(gameModel.Player.HPSprite);
            window.Draw(gameModel.Player.HPText);
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
