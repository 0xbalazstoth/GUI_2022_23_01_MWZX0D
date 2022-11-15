using Logic.Game;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Enums;
using Model.UI;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Dictionary<MovementDirection, Texture> enemyTextures;
        private Texture pistolTexture;
        private Texture shotgunTexture;
        private Texture hpTexture;
        private Texture gateTexture;

        public GameRenderer(IGameModel gameModel, string path)
        {
            this.gameModel = gameModel;
            this.assetsPath = path;

            // Note: Shouldn't be creating texture every frame, this is the reason of fps drops!
            bulletSheetTexture = new Texture("Assets/Textures/bullet_sheet.png");
            
            coinTexture = new Texture("Assets/Textures/coin_sheet.png");
            healthPotionTexture = new Texture("Assets/Textures/health_potion_sheet.png");
            speedPotionTexture = new Texture("Assets/Textures/speed_potion_sheet.png");
            
            playerTextures = new Dictionary<MovementDirection, Texture>();
            playerTextures.Add(MovementDirection.IdleRight, new Texture("Assets/Textures/idle_right.png"));
            playerTextures.Add(MovementDirection.IdleLeft, new Texture("Assets/Textures/idle_left.png"));
            playerTextures.Add(MovementDirection.Idle, new Texture("Assets/Textures/idle_right.png"));
            playerTextures.Add(MovementDirection.Left, new Texture("Assets/Textures/move_left.png"));
            playerTextures.Add(MovementDirection.Right, new Texture("Assets/Textures/move_right.png"));
            playerTextures.Add(MovementDirection.Up, new Texture("Assets/Textures/move_up.png"));
            playerTextures.Add(MovementDirection.Down, new Texture("Assets/Textures/move_down.png"));

            enemyTextures = new Dictionary<MovementDirection, Texture>();
            enemyTextures.Add(MovementDirection.Left, new Texture("Assets/Textures/enemy_eye_left.png"));
            enemyTextures.Add(MovementDirection.Right, new Texture("Assets/Textures/enemy_eye_right.png"));

            pistolTexture = new Texture("Assets/Textures/pistol.png");
            shotgunTexture = new Texture("Assets/Textures/shotgun.png");

            hpTexture = new Texture("Assets/Textures/heart.png");

            gateTexture = new Texture("Assets/Textures/gate.png");
        }

        public void Draw(RenderTarget window)
        {
            DrawTilemap(window);
            DrawObjects(window);
            if (gameModel.Player.PlayerState == Model.Game.Enums.GateState.InKillArena || gameModel.Player.PlayerState == Model.Game.Enums.GateState.InBossArena)
            {
                DrawCollectibleItems(window);
                DrawEnemy(window);
            }
            DrawPlayer(window);
            DrawBullets(window);

            if (gameModel.Player.IsDead)
            {
                window.Clear(Color.Black);
            }
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
            
            if (gameModel.Player.Gun.GunType == Model.Game.Enums.GunType.Shotgun)
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
                
                if (gameModel.Enemies[i].Gun.GunType == Model.Game.Enums.GunType.Shotgun)
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

            for (int i = 0; i < gameModel.Gates.Count; i++)
            {
                if (gameModel.Gates[i].IsGateReady)
                {
                    if (gameModel.DebugMode)
                    {
                        gameModel.Gates[i].Hitbox.FillColor = Color.Transparent;
                        gameModel.Gates[i].Hitbox.OutlineColor = Color.Red;
                        gameModel.Gates[i].Hitbox.OutlineThickness = 1.0f;

                        gameModel.Gates[i].InteractArea.FillColor = Color.Transparent;
                        gameModel.Gates[i].InteractArea.OutlineColor = Color.Green;
                        gameModel.Gates[i].InteractArea.OutlineThickness = 1.0f;

                        window.Draw(gameModel.Gates[i].Hitbox);
                        window.Draw(gameModel.Gates[i].InteractArea);
                    }

                    gameModel.Gates[i].Animations[0].Texture = gateTexture;
                    gameModel.Gates[i].Animations[0].Sprite = new Sprite(gameModel.Gates[i].Animations[0].Texture);
                    gameModel.Gates[i].Animations[0].TextureRect = new IntRect(0, 0, gameModel.Gates[i].Animations[0].GetSpriteSize.X, gameModel.Gates[i].Animations[0].GetSpriteSize.Y);
                    window.Draw(gameModel.Gates[i].GateSprite);

                    foreach (var gateText in gameModel.Gates[i].GateTexts)
                    {
                        window.Draw(gateText);
                    }
                }
            }
        }

        private void DrawEnemy(RenderTarget window)
        {
            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].Animations[MovementDirection.Left].Texture = enemyTextures[MovementDirection.Left];
                gameModel.Enemies[i].Animations[MovementDirection.Left].Sprite = new Sprite(gameModel.Enemies[i].Animations[MovementDirection.Left].Texture);
                gameModel.Enemies[i].Animations[MovementDirection.Left].TextureRect = new IntRect(0, 0, gameModel.Enemies[i].Animations[MovementDirection.Left].GetSpriteSize.X, gameModel.Enemies[i].Animations[MovementDirection.Left].GetSpriteSize.Y);

                gameModel.Enemies[i].Animations[MovementDirection.Right].Texture = enemyTextures[MovementDirection.Right];
                gameModel.Enemies[i].Animations[MovementDirection.Right].Sprite = new Sprite(gameModel.Enemies[i].Animations[MovementDirection.Right].Texture);
                gameModel.Enemies[i].Animations[MovementDirection.Right].TextureRect = new IntRect(0, 0, gameModel.Enemies[i].Animations[MovementDirection.Right].GetSpriteSize.X, gameModel.Enemies[i].Animations[MovementDirection.Right].GetSpriteSize.Y);

                if (gameModel.Enemies[i].Gun.GunType == Model.Game.Enums.GunType.Pistol)
                {
                    gameModel.Enemies[i].Gun.Texture = pistolTexture;
                    gameModel.Enemies[i].Gun.TextureRect = new IntRect(0, 0, 12, 3);
                }
                
                if (gameModel.Enemies[i].Gun.GunType == Model.Game.Enums.GunType.Shotgun)
                {
                    gameModel.Enemies[i].Gun.Texture = shotgunTexture;
                    gameModel.Enemies[i].Gun.TextureRect = new IntRect(0, 0, 16, 6);
                }

                gameModel.Enemies[i].Gun.Origin = new Vector2f(gameModel.Enemies[i].Gun.Texture.Size.X / 2f, gameModel.Enemies[i].Gun.Texture.Size.Y / 2f);

                if (gameModel.DebugMode)
                {
                    gameModel.Enemies[i].Hitbox.FillColor = Color.Transparent;
                    gameModel.Enemies[i].Hitbox.OutlineColor = Color.Red;
                    gameModel.Enemies[i].Hitbox.OutlineThickness = 1.0f;
                    window.Draw(gameModel.Enemies[i].Hitbox);
                }

                window.Draw(gameModel.Enemies[i]);
                gameModel.Enemies[i].HPSprite.Texture = hpTexture;
                
                if (gameModel.Player.PlayerState == GateState.InKillArena || gameModel.Player.PlayerState == GateState.InBossArena)
                {
                    window.Draw(gameModel.Enemies[i].Gun);

                    // Draw HP
                    window.Draw(gameModel.Enemies[i].HPSprite);
                    window.Draw(gameModel.Enemies[i].HPText);
                }
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

            if (gameModel.DebugMode)
            {
                gameModel.Player.Hitbox.FillColor = Color.Transparent;
                gameModel.Player.Hitbox.OutlineColor = Color.Red;
                gameModel.Player.Hitbox.OutlineThickness = 1.0f;
                window.Draw(gameModel.Player.Hitbox); 
            }

            window.Draw(gameModel.Player);
            window.Draw(gameModel.Player.Gun);

            // Draw HP
            gameModel.Player.HPSprite.Texture = hpTexture;
            window.Draw(gameModel.Player.HPSprite);
            window.Draw(gameModel.Player.HPText);
        }

        private void DrawTilemap(RenderTarget window)
        {
            for (int i = 0; i < gameModel.CurrentMap.Vertices.Count; i++)
            {
                window.Draw(gameModel.CurrentMap.Vertices[i], PrimitiveType.Quads, new(BlendMode.Alpha, Transform.Identity, gameModel.CurrentMap.TilesetTexture, null));
            }

            if (gameModel.Player.PlayerState == GateState.InLobby || gameModel.Player.PlayerState == GateState.InShop)
            {
                for (int i = 0; i < gameModel.CreatorTexts.Count; i++)
                {
                    window.Draw(gameModel.CreatorTexts[i]);
                }

                for (int i = 0; i < gameModel.SettingsTexts.Count; i++)
                {
                    window.Draw(gameModel.SettingsTexts[i]);
                }
            }
        }
    }
}
