using Logic.Game.Interfaces;
using Model.Game.Classes;
using Model.Game.Enums;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;

namespace Logic.Game.Classes
{
    public class BulletLogic : IBulletLogic
    {
        private IGameModel gameModel;
        private ITilemapLogic tilemapLogic;

        public BulletLogic(IGameModel gameModel, ITilemapLogic tilemapLogic)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;

            GunModel pistol = new GunModel();
            pistol.GunType = GunType.Pistol;
            pistol.Damage = 10;
            pistol.MaxAmmo = 15;
            pistol.Recoil = 5f;
            pistol.Scale = new Vector2f(2, 2);
            pistol.ShootSoundBuffer = new SoundBuffer("Assets/Sounds/pistol.ogg");
            pistol.ShootSound = new Sound(pistol.ShootSoundBuffer);
            pistol.EmptySoundBuffer = new SoundBuffer("Assets/Sounds/gun_empty.ogg");
            pistol.EmptySound = new Sound(pistol.EmptySoundBuffer);
            pistol.FiringInterval = TimeSpan.FromMilliseconds(300);
            pistol.CurrentAmmo = pistol.MaxAmmo;
            pistol.ReloadSoundBuffer = new("Assets/Sounds/gun_reload.ogg");
            pistol.ReloadSound = new(pistol.ReloadSoundBuffer);
            pistol.ShootSounds = new List<Sound>();

            GunModel shotgun = new GunModel();
            shotgun.GunType = GunType.Shotgun;
            shotgun.Damage = 20;
            shotgun.MaxAmmo = 5;
            shotgun.Recoil = 10f;
            shotgun.Scale = new Vector2f(2, 2);
            shotgun.ShootSoundBuffer = new SoundBuffer("Assets/Sounds/pistol.ogg");
            shotgun.ShootSound = new Sound(shotgun.ShootSoundBuffer);
            shotgun.EmptySoundBuffer = new SoundBuffer("Assets/Sounds/gun_empty.ogg");
            shotgun.EmptySound = new Sound(shotgun.EmptySoundBuffer);
            shotgun.FiringInterval = TimeSpan.FromMilliseconds(750);
            shotgun.CurrentAmmo = shotgun.MaxAmmo;
            shotgun.ReloadSoundBuffer = new("Assets/Sounds/gun_reload.ogg");
            shotgun.ReloadSound = new(shotgun.ReloadSoundBuffer);
            shotgun.ShootSounds = new List<Sound>();

            gameModel.Guns = new List<GunModel>();
            gameModel.Guns.Add(pistol);
            gameModel.Guns.Add(shotgun);
        }

        public void HandlePlayerBulletMapCollision(RenderWindow window)
        {
            foreach (BulletModel bullet in gameModel.Player.Gun.Bullets)
            {
                var xTileposition = bullet.Bullet.Position.X;
                var yTileposition = bullet.Bullet.Position.Y;
                var tilePosition = new Vector2i((int)((int)xTileposition / gameModel.CurrentMap.TileSize.X), (int)((int)yTileposition / gameModel.CurrentMap.TileSize.Y));

                if (tilePosition.X < 0 || tilePosition.X > gameModel.CurrentMap.Size.X || tilePosition.Y < 0 || tilePosition.Y > gameModel.CurrentMap.Size.Y)
                {
                    gameModel.Player.Gun.Bullets.Remove(bullet);
                    return;
                }

                var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                if (gameModel.CurrentMap.CollidableIDs.Contains(currentTileID) == false)
                {
                    continue;
                }

                var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.CurrentMap.TileSize.X, gameModel.CurrentMap.TileSize.Y);
                var rect = bullet.Bullet.GetGlobalBounds();

                if (rect.Intersects(tileRect))
                {
                    gameModel.Player.Gun.Bullets.Remove(bullet);
                    return;
                }
            }
        }

        public void HandlePlayerBulletObjectCollision(Sprite item)
        {
            foreach (var bullet in gameModel.Player.Gun.Bullets)
            {
                if (bullet.Bullet.GetGlobalBounds().Intersects(item.GetGlobalBounds()))
                {
                    gameModel.Player.Gun.Bullets.Remove(bullet);
                    return;
                }
            }
        }

        public void UpdatePlayerBullets()
        {
            for (int i = 0; i < gameModel.Player.Gun.Bullets.Count; i++)
            {
                gameModel.Player.Gun.Bullets[i].Bullet.Position += gameModel.Player.Gun.Bullets[i].Velocity;

                float distX = gameModel.Player.Gun.Bullets[i].Bullet.Position.X - gameModel.Player.Center.X;
                float distY = gameModel.Player.Gun.Bullets[i].Bullet.Position.Y - gameModel.Player.Center.Y;

                if (Math.Sqrt(distX * distX + distY * distY) > 600)
                {
                    gameModel.Player.Gun.Bullets.RemoveAt(i);
                }
            }
        }

        public void UpdateEnemiesBullets()
        {
            foreach (EnemyModel enemy in gameModel.Enemies)
            {
                for (int i = 0; i < enemy.Gun.Bullets.Count; i++)
                {
                    enemy.Gun.Bullets[i].Bullet.Position += enemy.Gun.Bullets[i].Velocity;

                    float distX = enemy.Gun.Bullets[i].Bullet.Position.X - gameModel.Player.Center.X;
                    float distY = enemy.Gun.Bullets[i].Bullet.Position.Y - gameModel.Player.Center.Y;

                    if (Math.Sqrt(distX * distX + distY * distY) > gameModel.CurrentMap.GetMapWidth)
                    {
                        enemy.Gun.Bullets.RemoveAt(i);
                    }
                }
            }
        }

        public void UpdatePlayerBulletAnimationTextures()
        {
            for (int i = 0; i < gameModel.Player.Gun.Bullets.Count; i++)
            {
                gameModel.Player.Gun.Bullets[i].Bullet.Texture = gameModel.Player.Gun.Bullets[i].Animations[gameModel.Player.Gun.GunType].Texture;
                gameModel.Player.Gun.Bullets[i].Bullet.TextureRect = gameModel.Player.Gun.Bullets[i].Animations[gameModel.Player.Gun.GunType].TextureRect;
            }
        }

        public void HandleEnemiesBulletMapCollision(RenderWindow window)
        {
            foreach (EnemyModel enemy in gameModel.Enemies)
            {
                foreach (BulletModel bullet in enemy.Gun.Bullets)
                {
                    var xTileposition = bullet.Bullet.Position.X;
                    var yTileposition = bullet.Bullet.Position.Y;
                    var tilePosition = new Vector2i((int)((int)xTileposition / gameModel.CurrentMap.TileSize.X), (int)((int)yTileposition / gameModel.CurrentMap.TileSize.Y));

                    if (tilePosition.X < 0 || tilePosition.X > gameModel.CurrentMap.Size.X || tilePosition.Y < 0 || tilePosition.Y > gameModel.CurrentMap.Size.Y)
                    {
                        enemy.Gun.Bullets.Remove(bullet);
                        return;
                    }

                    var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                    if (gameModel.CurrentMap.CollidableIDs.Contains(currentTileID) == false)
                    {
                        continue;
                    }

                    var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                    var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.CurrentMap.TileSize.X, gameModel.CurrentMap.TileSize.Y);
                    var rect = bullet.Bullet.GetGlobalBounds();

                    if (rect.Intersects(tileRect))
                    {
                        enemy.Gun.Bullets.Remove(bullet);
                        return;
                    }
                }
            }
        }

        public void HandleEnemiesBulletObjectCollision(Sprite item)
        {
            foreach (EnemyModel enemy in gameModel.Enemies)
            {
                foreach (BulletModel bullet in enemy.Gun.Bullets)
                {
                    if (bullet.Bullet.GetGlobalBounds().Intersects(item.GetGlobalBounds()))
                    {
                        enemy.Gun.Bullets.Remove(bullet);
                        return;
                    }
                }
            }
        }

        public void UpdateEnemiesBulletAnimationTextures()
        {
            foreach (EnemyModel enemy in gameModel.Enemies)
            {
                for (int i = 0; i < enemy.Gun.Bullets.Count; i++)
                {
                    enemy.Gun.Bullets[i].Bullet.Texture = enemy.Gun.Bullets[i].Animations[enemy.Gun.GunType].Texture;
                    enemy.Gun.Bullets[i].Bullet.TextureRect = enemy.Gun.Bullets[i].Animations[enemy.Gun.GunType].TextureRect;
                }
            }
        }
    }
}
