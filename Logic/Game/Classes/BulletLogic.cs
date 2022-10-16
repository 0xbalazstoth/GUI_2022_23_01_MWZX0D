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
            pistol.Scale = new Vector2f(2, 2);
            pistol.ShootSoundBuffer = new SoundBuffer("Assets/Sounds/pistol.ogg");
            pistol.ShootSound = new Sound(pistol.ShootSoundBuffer);
            pistol.EmptySoundBuffer = new SoundBuffer("Assets/Sounds/gun_empty.ogg");
            pistol.EmptySound = new Sound(pistol.EmptySoundBuffer);
            pistol.FiringInterval = TimeSpan.FromMilliseconds(300);
            pistol.CurrentAmmo = pistol.MaxAmmo;
            pistol.ReloadSoundBuffer = new("Assets/Sounds/gun_reload.ogg");
            pistol.ReloadSound = new(pistol.ReloadSoundBuffer);

            GunModel shotgun = new GunModel();
            shotgun.GunType = GunType.Shotgun;
            shotgun.Damage = 20;
            shotgun.MaxAmmo = 5;
            shotgun.Scale = new Vector2f(2, 2);
            shotgun.ShootSoundBuffer = new SoundBuffer("Assets/Sounds/pistol.ogg");
            shotgun.ShootSound = new Sound(shotgun.ShootSoundBuffer);
            shotgun.EmptySoundBuffer = new SoundBuffer("Assets/Sounds/gun_empty.ogg");
            shotgun.EmptySound = new Sound(shotgun.EmptySoundBuffer);
            shotgun.FiringInterval = TimeSpan.FromMilliseconds(750);
            shotgun.CurrentAmmo = shotgun.MaxAmmo;
            shotgun.ReloadSoundBuffer = new("Assets/Sounds/gun_reload.ogg");
            shotgun.ReloadSound = new(shotgun.ReloadSoundBuffer);


            gameModel.Guns = new List<GunModel>();
            gameModel.Guns.Add(pistol);
            gameModel.Guns.Add(shotgun);
        }

        public void HandleMapCollision(RenderWindow window)
        {
            foreach (BulletModel bullet in gameModel.Player.Gun.Bullets)
            {
                var xTileposition = bullet.Bullet.Position.X;
                var yTileposition = bullet.Bullet.Position.Y;
                var tilePosition = new Vector2i((int)((int)xTileposition / gameModel.Map.TileSize.X), (int)((int)yTileposition / gameModel.Map.TileSize.Y));

                if (tilePosition.X < 0 || tilePosition.X > gameModel.Map.Size.X || tilePosition.Y < 0 || tilePosition.Y > gameModel.Map.Size.Y)
                {
                    gameModel.Player.Gun.Bullets.Remove(bullet);
                    return;
                }

                var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                if (gameModel.Map.CollidableIDs.Contains(currentTileID) == false)
                {
                    continue;
                }

                var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.Map.TileSize.X, gameModel.Map.TileSize.Y);
                var rect = bullet.Bullet.GetGlobalBounds();

                if (rect.Intersects(tileRect))
                {
                    gameModel.Player.Gun.Bullets.Remove(bullet);
                    return;
                }
            }
        }

        public void HandleObjectCollision(Sprite item)
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

        public void Shoot()
        {
            BulletModel tempBullet = new BulletModel();
            tempBullet.Bullet = new Sprite();
            tempBullet.Speed = 15f;
            tempBullet.Bullet.Position = gameModel.Player.Gun.Position;
            tempBullet.Velocity = gameModel.Player.AimDirectionNormalized * tempBullet.Speed;
            tempBullet.Bullet.Origin = new Vector2f(tempBullet.Bullet.TextureRect.Width / 2, tempBullet.Bullet.TextureRect.Height / 2);
            tempBullet.Bullet.Scale = new Vector2f(0.5f, 0.5f);

            tempBullet.Animations = new Dictionary<GunType, AnimationModel>();

            if (gameModel.Player.Gun.GunType == GunType.Pistol)
            {
                tempBullet.Animations.Add(GunType.Pistol, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 10f,
                });
            }
            else if (gameModel.Player.Gun.GunType == GunType.Shotgun)
            {
                tempBullet.Animations.Add(GunType.Shotgun, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 10f,
                });
            }

            // Player can shoot every 1 seconds
            if (gameModel.Player.Gun.LastFired + gameModel.Player.Gun.FiringInterval < DateTime.Now)
            {
                // Check if player has ammo based on max ammo
                if (gameModel.Player.Gun.CurrentAmmo > 0 && (gameModel.Player.Gun.CurrentAmmo <= gameModel.Player.Gun.MaxAmmo))
                {
                    gameModel.Player.Gun.Bullets.Add(tempBullet);
                    gameModel.Player.Gun.CurrentAmmo--;
                    gameModel.Player.Gun.LastFired = DateTime.Now;

                    if (gameModel.Player.Gun.ShootSound.Status == SoundStatus.Stopped)
                    {
                        gameModel.Player.Gun.ShootSound.Play();
                    }

                    Trace.WriteLine(gameModel.Player.Gun.CurrentAmmo);
                }
                else
                {
                    // Reload needed, gun is empty
                    if (gameModel.Player.Gun.EmptySound.Status == SoundStatus.Stopped)
                    {
                        gameModel.Player.Gun.EmptySound.Play();
                    }
                }
            }

            // Shake camera
            if (gameModel.Player.Gun.CurrentAmmo > 0)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Center.X + (float)new Random().NextDouble() * 10f - 5f, gameModel.CameraView.Center.Y + (float)new Random().NextDouble() * 10f - 5f);
            }
        }

        public void Update()
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

        public void UpdateBulletAnimationTextures()
        {
            for (int i = 0; i < gameModel.Player.Gun.Bullets.Count; i++)
            {
                gameModel.Player.Gun.Bullets[i].Bullet.Texture = gameModel.Player.Gun.Bullets[i].Animations[gameModel.Player.Gun.GunType].Texture;
                gameModel.Player.Gun.Bullets[i].Bullet.TextureRect = gameModel.Player.Gun.Bullets[i].Animations[gameModel.Player.Gun.GunType].TextureRect;
            }
        }
    }
}
