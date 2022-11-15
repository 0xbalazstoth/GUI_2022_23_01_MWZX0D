using Logic.Game.Interfaces;
using Model.Game.Classes;
using Model.Game.Enums;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Classes
{
    public class EnemyLogic : IEnemyLogic
    {
        private IGameModel gameModel;
        private ITilemapLogic tilemapLogic;

        public EnemyLogic(IGameModel gameModel, ITilemapLogic tilemapLogic)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;

            gameModel.Enemies = new List<EnemyModel>();

            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].PreviousPosition = gameModel.Enemies[i].Position;
            }
        }

        public void PathToPlayer(int enemyIdx)
        {
            // Enemy stay inside the map
            if (gameModel.Enemies[enemyIdx].Position.X < 0)
            {
                gameModel.Enemies[enemyIdx].Position = new Vector2f(0, gameModel.Enemies[enemyIdx].Position.Y);
            }
            if (gameModel.Enemies[enemyIdx].Position.X > gameModel.CurrentMap.Width * gameModel.CurrentMap.TileSize.X)
            {
                gameModel.Enemies[enemyIdx].Position = new Vector2f(gameModel.CurrentMap.Width * gameModel.CurrentMap.TileSize.X, gameModel.Enemies[enemyIdx].Position.Y);
            }
            if (gameModel.Enemies[enemyIdx].Position.Y < 0)
            {
                gameModel.Enemies[enemyIdx].Position = new Vector2f(gameModel.Enemies[enemyIdx].Position.X, 0);
            }
            if (gameModel.Enemies[enemyIdx].Position.Y > gameModel.CurrentMap.Height * gameModel.CurrentMap.TileSize.Y)
            {
                gameModel.Enemies[enemyIdx].Position = new Vector2f(gameModel.Enemies[enemyIdx].Position.X, gameModel.CurrentMap.Height * gameModel.CurrentMap.TileSize.Y);
            }

            int[] grid = gameModel.CurrentMap.MapLayers[1];

            gameModel.Enemies[enemyIdx].Path = new List<Vector2i>();

            Vector2i start = new Vector2i((int)gameModel.Player.Position.X / 32, (int)gameModel.Player.Position.Y / 32);
            Vector2i end = new Vector2i((int)gameModel.Enemies[enemyIdx].Position.X / 32, (int)gameModel.Enemies[enemyIdx].Position.Y / 32);

            // add the start point to the list
            gameModel.Enemies[enemyIdx].Path.Add(start);

            if (gameModel.Enemies[enemyIdx].Path[gameModel.Enemies[enemyIdx].Path.Count - 1] != end)
            {
                // get the last point in the list
                Vector2i last = gameModel.Enemies[enemyIdx].Path[gameModel.Enemies[enemyIdx].Path.Count - 1];

                // get the adjacent points
                List<Vector2i> adjacent = new List<Vector2i>();
                adjacent.Add(new Vector2i(last.X, last.Y - 1));
                adjacent.Add(new Vector2i(last.X + 1, last.Y));
                adjacent.Add(new Vector2i(last.X, last.Y + 1));
                adjacent.Add(new Vector2i(last.X - 1, last.Y));

                // for each adjacent point
                foreach (Vector2i point in adjacent)
                {
                    // check if the point is in the grid
                    if (point.X >= 0 && point.X < gameModel.CurrentMap.Width && point.Y >= 0 && point.Y < gameModel.CurrentMap.Height)
                    {
                        // check if the point is not a wall
                        foreach (var collidibleId in gameModel.CurrentMap.CollidableIDs)
                        {
                            if (grid[point.X + point.Y * gameModel.CurrentMap.Width] != collidibleId)
                            {
                                // check if the point is not already in the list
                                if (!gameModel.Enemies[enemyIdx].Path.Contains(point))
                                {
                                    // add the point to the list
                                    gameModel.Enemies[enemyIdx].Path.Add(point);
                                }
                            }
                        }
                    }
                }
            }

            // Create copy of the path
            List<Vector2i> pathCopy = new List<Vector2i>();

            for (int j = 0; j < gameModel.Enemies[enemyIdx].Path.Count; j++)
            {
                pathCopy.Add(gameModel.Enemies[enemyIdx].Path[j]);
            }

            // Remove the first point in the path
            pathCopy.RemoveAt(0);

            // Move enemy
            if (pathCopy.Count > 0)
            {
                gameModel.Enemies[enemyIdx].PreviousPosition = gameModel.Enemies[enemyIdx].Position;

                // left, right
                if (pathCopy[0].X < gameModel.Enemies[enemyIdx].Position.X / gameModel.CurrentMap.TileWidth)
                {
                    gameModel.Enemies[enemyIdx].Position = new Vector2f(gameModel.Enemies[enemyIdx].Position.X - 1, gameModel.Enemies[enemyIdx].Position.Y);
                }
                else if (pathCopy[0].X > gameModel.Enemies[enemyIdx].Position.X / gameModel.CurrentMap.TileWidth)
                {
                    gameModel.Enemies[enemyIdx].Position = new Vector2f(gameModel.Enemies[enemyIdx].Position.X + 1, gameModel.Enemies[enemyIdx].Position.Y);
                }

                // up, down
                if (pathCopy[0].Y < gameModel.Enemies[enemyIdx].Position.Y / gameModel.CurrentMap.TileHeight)
                {
                    gameModel.Enemies[enemyIdx].Position = new Vector2f(gameModel.Enemies[enemyIdx].Position.X, gameModel.Enemies[enemyIdx].Position.Y - 1);
                }
                else if (pathCopy[0].Y > gameModel.Enemies[enemyIdx].Position.Y / gameModel.CurrentMap.TileHeight)
                {
                    gameModel.Enemies[enemyIdx].Position = new Vector2f(gameModel.Enemies[enemyIdx].Position.X, gameModel.Enemies[enemyIdx].Position.Y + 1);
                }

                HandleMapCollision(enemyIdx);

                var currentRect = new FloatRect(gameModel.Enemies[enemyIdx].Position.X, gameModel.Enemies[enemyIdx].Position.Y, gameModel.Enemies[enemyIdx].Hitbox.Size.X, gameModel.Enemies[enemyIdx].Hitbox.Size.Y);

                // Check if current enemy colliding with other enemies
                for (int i = 0; i < gameModel.Enemies.Count; i++)
                {
                    if (i != enemyIdx)
                    {
                        var otherRect = new FloatRect(gameModel.Enemies[i].Position.X, gameModel.Enemies[i].Position.Y, gameModel.Enemies[i].Hitbox.Size.X, gameModel.Enemies[i].Hitbox.Size.Y);

                        if (currentRect.Intersects(otherRect))
                        {
                            gameModel.Enemies[enemyIdx].Position = gameModel.Enemies[enemyIdx].PreviousPosition;
                        }
                    }
                }
            }

            // Clear the path
            gameModel.Enemies[enemyIdx].Path.Clear();
        }

        public void HandleBulletCollision()
        {
            foreach (var bullet in gameModel.Player.Gun.Bullets.ToList())
            {
                foreach (var enemy in gameModel.Enemies)
                {
                    if (bullet.Bullet.GetGlobalBounds().Intersects(enemy.GetGlobalBounds()))
                    {
                        gameModel.Player.Gun.Bullets.Remove(bullet);

                        // Damage enemy if it is not dead
                        if (enemy.CurrentHP >= 1)
                        {
                            enemy.CurrentHP -= gameModel.Player.Gun.Damage;
                        }

                        if (enemy.CurrentHP <= 0)
                        {
                            gameModel.Enemies.Remove(enemy);
                            gameModel.Player.CurrentXP += enemy.RewardXP;
                            gameModel.Player.KillCounter++;
                        }

                        break;
                    }
                }
            }
        }

        public void UpdateAnimationTextures()
        {
            // Update animation textures
            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].Texture = gameModel.Enemies[i].Animations[Model.Game.MovementDirection.Left].Texture;
                gameModel.Enemies[i].TextureRect = gameModel.Enemies[i].Animations[Model.Game.MovementDirection.Left].TextureRect;
            }
        }

        public void UpdateHP()
        {
            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].HPSprite.Position = new Vector2f(gameModel.Enemies[i].Position.X + gameModel.Enemies[i].TextureRect.Width / 2 - gameModel.Enemies[i].HPSprite.TextureRect.Width, gameModel.Enemies[i].Position.Y - gameModel.Enemies[i].HPSprite.TextureRect.Height);
                gameModel.Enemies[i].HPText.Position = new Vector2f(gameModel.Enemies[i].HPSprite.Position.X + 18f, gameModel.Enemies[i].HPSprite.Position.Y - (gameModel.Enemies[i].HPSprite.GetGlobalBounds().Height / 2f) + 4f);

                gameModel.Enemies[i].HPText.DisplayedString = $"{gameModel.Enemies[i].CurrentHP}";
            }
        }

        public void Shoot(int enemyIdx)
        {
            gameModel.Enemies[enemyIdx].AimDirection = new Vector2f(gameModel.Player.Position.X - gameModel.Enemies[enemyIdx].Position.X, gameModel.Player.Position.Y - gameModel.Enemies[enemyIdx].Position.Y);
            gameModel.Enemies[enemyIdx].AimDirectionNormalized = gameModel.Enemies[enemyIdx].AimDirection / (float)Math.Sqrt(gameModel.Enemies[enemyIdx].AimDirection.X * gameModel.Enemies[enemyIdx].AimDirection.X + gameModel.Enemies[enemyIdx].AimDirection.Y * gameModel.Enemies[enemyIdx].AimDirection.Y);

            if (gameModel.Enemies[enemyIdx].Gun.LastFired + gameModel.Enemies[enemyIdx].Gun.FiringInterval < DateTime.Now)
            {
                if (gameModel.Enemies[enemyIdx].Gun.CurrentAmmo > 0 && (gameModel.Enemies[enemyIdx].Gun.CurrentAmmo <= gameModel.Enemies[enemyIdx].Gun.MaxAmmo))
                {
                    BulletModel tempBullet = new BulletModel();
                    tempBullet.Bullet = new Sprite();
                    tempBullet.Speed = 5f;
                    tempBullet.Bullet.Position = gameModel.Enemies[enemyIdx].Position;
                    tempBullet.Velocity = gameModel.Enemies[enemyIdx].AimDirectionNormalized * tempBullet.Speed;
                    tempBullet.Bullet.Origin = new Vector2f(tempBullet.Bullet.TextureRect.Width / 2, tempBullet.Bullet.TextureRect.Height / 2);
                    tempBullet.Bullet.Scale = new Vector2f(0.5f, 0.5f);

                    tempBullet.Animations = new Dictionary<GunType, AnimationModel>();
                    tempBullet.Animations.Add(GunType.Pistol, new AnimationModel()
                    {
                        Row = 0,
                        ColumnsInRow = 8,
                        TotalRows = 1,
                        TotalColumns = 8,
                        Speed = 10f,
                    });

                    gameModel.Enemies[enemyIdx].Gun.CurrentAmmo--;
                    gameModel.Enemies[enemyIdx].Gun.LastFired = DateTime.Now;
                    gameModel.Enemies[enemyIdx].Gun.Bullets.Add(tempBullet);
                }
                else
                {
                    // Reload needed, because ammo is empty, but not yet reloaded (reload time is x seconds)
                    if (gameModel.Enemies[enemyIdx].Gun.LastReloaded + gameModel.Enemies[enemyIdx].Gun.ReloadTime < DateTime.Now)
                    {
                        ReloadGun(enemyIdx);
                        gameModel.Enemies[enemyIdx].Gun.LastReloaded = DateTime.Now;
                    }
                }
            }
        }

        public void FlipAndRotateGun()
        {
            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                // Rotate gun
                var angle = (float)(Math.Atan2(gameModel.Player.Position.Y - gameModel.Enemies[i].Position.Y, gameModel.Player.Position.X - gameModel.Enemies[i].Position.X) * 180 / Math.PI);

                // Change enemy animation texture by aim direction
                if (angle > -45 && angle < 45)
                {
                    gameModel.Enemies[i].Texture = gameModel.Enemies[i].Animations[Model.Game.MovementDirection.Right].Texture;
                    gameModel.Enemies[i].TextureRect = gameModel.Enemies[i].Animations[Model.Game.MovementDirection.Right].TextureRect;
                }
                else if (angle > 135 || angle < -135)
                {
                    gameModel.Enemies[i].Texture = gameModel.Enemies[i].Animations[Model.Game.MovementDirection.Left].Texture;
                    gameModel.Enemies[i].TextureRect = gameModel.Enemies[i].Animations[Model.Game.MovementDirection.Left].TextureRect;
                }

                // Flip gun
                if (angle > 90 || angle < -90)
                {
                    gameModel.Enemies[i].Gun.Scale = new Vector2f(-2.5f, 2.5f);
                    gameModel.Enemies[i].Gun.Rotation = angle + 180;

                    if (gameModel.Enemies[i].EnemyType == EnemyType.Eye)
                    {
                        // Center gun position by enemy texture
                        gameModel.Enemies[i].Gun.Position = new Vector2f(gameModel.Enemies[i].Position.X + (gameModel.Enemies[i].TextureRect.Width / 2f), gameModel.Enemies[i].Position.Y + (gameModel.Enemies[i].TextureRect.Height / 2f));
                    }
                }
                else
                {
                    gameModel.Enemies[i].Gun.Scale = new Vector2f(2.5f, 2.5f);
                    gameModel.Enemies[i].Gun.Rotation = angle;

                    if (gameModel.Enemies[i].EnemyType == EnemyType.Eye)
                    {
                        // Center gun position by enemy texture
                        gameModel.Enemies[i].Gun.Position = new Vector2f(gameModel.Enemies[i].Position.X + (gameModel.Enemies[i].TextureRect.Width / 2f), gameModel.Enemies[i].Position.Y + (gameModel.Enemies[i].TextureRect.Height / 2f));
                    }
                }
            }
        }

        public void CreateEnemies(EnemyType enemyType, int damage, int maxAmmo, int spawnCount)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                EnemyModel enemy = new EnemyModel();
                enemy.Position = new Vector2f(new Random().Next(400, (int)(gameModel.CurrentMap.GetMapWidth - gameModel.CurrentMap.TileWidth)), new Random().Next(400, (int)(gameModel.CurrentMap.GetMapHeight - gameModel.CurrentMap.TileHeight)));
                enemy.Speed = 30f;
                enemy.SightDistance = 300f;
                enemy.Gun = new GunModel();
                enemy.Gun.GunType = Model.Game.Enums.GunType.Pistol;
                enemy.Gun.Damage = damage;
                enemy.Gun.MaxAmmo = maxAmmo;
                enemy.Gun.Recoil = 5f;
                enemy.Hitbox = new RectangleShape();
                enemy.Gun.ReloadTime = TimeSpan.FromSeconds(5);
                enemy.Gun.Scale = new Vector2f(2, 2);
                enemy.Gun.ShootSoundBuffer = new SoundBuffer("Assets/Sounds/pistol.ogg");
                enemy.Gun.ShootSound = new Sound(enemy.Gun.ShootSoundBuffer);
                enemy.Gun.EmptySoundBuffer = new SoundBuffer("Assets/Sounds/gun_empty.ogg");
                enemy.Gun.EmptySound = new Sound(enemy.Gun.EmptySoundBuffer);
                enemy.Gun.FiringInterval = TimeSpan.FromMilliseconds(750);
                enemy.Gun.CurrentAmmo = enemy.Gun.MaxAmmo;
                enemy.Gun.ReloadSoundBuffer = new("Assets/Sounds/gun_reload.ogg");
                enemy.Gun.ReloadSound = new Sound(enemy.Gun.ReloadSoundBuffer);
                enemy.Gun.ShootSounds = new List<Sound>();
                enemy.HPSprite = new Sprite();
                enemy.HPText = new Text();
                enemy.CurrentHP = enemy.MaxHP;
                enemy.HPText.CharacterSize = 16;
                enemy.HPText.FillColor = Color.Red;

                enemy.Gun.Bullets = new List<BulletModel>();
                enemy.RewardXP = new Random().Next(2, 11);
                enemy.EnemyType = enemyType;

                gameModel.Enemies.Add(enemy);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.Enemies[i].GetGlobalBounds().Intersects(gameModel.Enemies[j].GetGlobalBounds()))
                    {
                        gameModel.Enemies.RemoveAt(i);
                        j = 0;
                    }
                }
            }
        }

        public void HandleMapCollision(int enemyIdx)
        {
            // Check if enemy is on a collidable tile
            for (int y = -2; y < 2; y++)
            {
                for (int x = -2; x < 2; x++)
                {
                    var xTilePosition = gameModel.Enemies[enemyIdx].Position.X;
                    var yTilePosition = gameModel.Enemies[enemyIdx].Position.Y;
                    var currentTilePosition = new Vector2i((int)((int)xTilePosition / gameModel.CurrentMap.TileSize.X), (int)((int)yTilePosition / gameModel.CurrentMap.TileSize.Y)) + new Vector2i(x, y);
                    var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, currentTilePosition.X, currentTilePosition.Y);

                    if (gameModel.CurrentMap.CollidableIDs.Contains(currentTileID) == false)
                    {
                        continue;
                    }

                    var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(currentTilePosition.X, currentTilePosition.Y);
                    var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.CurrentMap.TileSize.X, gameModel.CurrentMap.TileSize.Y);
                    var rect = new FloatRect(gameModel.Enemies[enemyIdx].Position.X, gameModel.Enemies[enemyIdx].Position.Y, gameModel.Enemies[enemyIdx].Hitbox.Size.X, gameModel.Enemies[enemyIdx].Hitbox.Size.Y);

                    if (tileRect.Intersects(rect))
                    {
                        gameModel.Enemies[enemyIdx].Position = gameModel.Enemies[enemyIdx].PreviousPosition;
                        return;
                    }
                }
            }
        }

        public void ReloadGun(int enemyIdx)
        {
            if (gameModel.Enemies[enemyIdx].Gun.CurrentAmmo == 0)
            {
                gameModel.Enemies[enemyIdx].Gun.CurrentAmmo = gameModel.Enemies[enemyIdx].Gun.MaxAmmo;
            }
            else if (gameModel.Enemies[enemyIdx].Gun.CurrentAmmo < gameModel.Enemies[enemyIdx].Gun.MaxAmmo)
            {
                gameModel.Enemies[enemyIdx].Gun.CurrentAmmo = gameModel.Enemies[enemyIdx].Gun.MaxAmmo;
            }
        }

        public float DistanceBetweenPlayer(int enemyIdx)
        {
            // Calculate distance between enemy and player
            float distance = (float)Math.Sqrt(Math.Pow(gameModel.Enemies[enemyIdx].Position.X - gameModel.Player.Position.X, 2) + Math.Pow(gameModel.Enemies[enemyIdx].Position.Y - gameModel.Player.Position.Y, 2));

            return distance;
        }
    }
}
