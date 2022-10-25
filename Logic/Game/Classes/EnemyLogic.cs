﻿using Logic.Game.Interfaces;
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

            CreateEnemies();

            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].HPSprite = new Sprite();
                gameModel.Enemies[i].HPSprite.Position = new Vector2f(gameModel.Enemies[i].Position.X, gameModel.Enemies[i].Position.Y);

                gameModel.Enemies[i].CurrentHP = gameModel.Enemies[i].MaxHP;

                gameModel.Enemies[i].HPText = new Text();
                gameModel.Enemies[i].HPText.Position = new Vector2f(gameModel.Enemies[i].Position.X, gameModel.Enemies[i].Position.Y);
                gameModel.Enemies[i].HPText.CharacterSize = 16;
                gameModel.Enemies[i].HPText.FillColor = Color.Red;
            }
        }

        public void PathToPlayer()
        {
            // Algorithms for pathfinding
            // 1. Dijkstra's algorithm
            // 2. A* algorithm
            // 3. Breadth-first search
            // 4. Depth-first search

            // Find path to the player
            foreach (var enemy in gameModel.Enemies)
            {
                if (gameModel.Player.Position.X < enemy.Position.X)
                {
                    enemy.Position = new Vector2f(enemy.Position.X - 1, enemy.Position.Y);
                }
                else if (gameModel.Player.Position.X > enemy.Position.X)
                {
                    enemy.Position = new Vector2f(enemy.Position.X + 1, enemy.Position.Y);
                }

                if (gameModel.Player.Position.Y < enemy.Position.Y)
                {
                    enemy.Position = new Vector2f(enemy.Position.X, enemy.Position.Y - 1);
                }
                else if (gameModel.Player.Position.Y > enemy.Position.Y)
                {
                    enemy.Position = new Vector2f(enemy.Position.X, enemy.Position.Y + 1);
                }
            }
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

                        if (enemy.CurrentHP == 0)
                        {
                            gameModel.Enemies.Remove(enemy);
                            gameModel.Player.CurrentXP += enemy.RewardXP;
                        }

                        break;
                    }
                }
            }
        }

        public void HandleMovement()
        {
            // Previous positions
            //for (int i = 0; i < gameModel.Enemies.Count; i++)
            //{
            //    previousPositions[i] = gameModel.Enemies[i].Position;
            //}
        }

        public void LoadTexture(string filename)
        {

        }

        public void LoadTexture(Texture texture)
        {

        }

        public void UpdateAnimationTextures(float dt, Texture[] texture, IntRect[] textureRect)
        {

        }

        public void UpdateDeltaTime(float dt)
        {

        }

        public void UpdateHP()
        {
            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].HPSprite.Position = new Vector2f(gameModel.Enemies[i].Position.X - (gameModel.Enemies[i].GetGlobalBounds().Width / 2f), gameModel.Enemies[i].Position.Y - (gameModel.Enemies[i].GetGlobalBounds().Height / 2f));
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
                    tempBullet.Speed = 4f;
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
                // Rotate fun
                var angle = (float)(Math.Atan2(gameModel.Player.Position.Y - gameModel.Enemies[i].Position.Y, gameModel.Player.Position.X - gameModel.Enemies[i].Position.X) * 180 / Math.PI);

                // Flip gun
                if (angle > 90 || angle < -90)
                {
                    gameModel.Enemies[i].Gun.Scale = new Vector2f(-2.5f, 2.5f);
                    gameModel.Enemies[i].Gun.Rotation = angle + 180;
                    gameModel.Enemies[i].Gun.Position = new Vector2f(gameModel.Enemies[i].Position.X - 10, gameModel.Enemies[i].Position.Y + 5);
                }
                else
                {
                    gameModel.Enemies[i].Gun.Scale = new Vector2f(2.5f, 2.5f);
                    gameModel.Enemies[i].Gun.Rotation = angle;
                    gameModel.Enemies[i].Gun.Position = new Vector2f(gameModel.Enemies[i].Position.X + 10, gameModel.Enemies[i].Position.Y + 5);
                }
            }
        }

        public void CreateEnemies()
        {
            for (int i = 0; i < 4; i++)
            {
                EnemyModel enemy = new EnemyModel();
                enemy.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                enemy.Speed = 30f;
                enemy.Gun = new GunModel();
                enemy.Gun.GunType = Model.Game.Enums.GunType.Pistol;
                enemy.Gun.Damage = 10;
                enemy.Gun.MaxAmmo = 15;
                enemy.Gun.Recoil = 5f;
                enemy.Gun.ReloadTime = TimeSpan.FromSeconds(15);
                enemy.Gun.Scale = new Vector2f(2, 2);
                enemy.Gun.ShootSoundBuffer = new SoundBuffer("Assets/Sounds/pistol.ogg");
                enemy.Gun.ShootSound = new Sound(enemy.Gun.ShootSoundBuffer);
                enemy.Gun.EmptySoundBuffer = new SoundBuffer("Assets/Sounds/gun_empty.ogg");
                enemy.Gun.EmptySound = new Sound(enemy.Gun.EmptySoundBuffer);
                enemy.Gun.FiringInterval = TimeSpan.FromMilliseconds(300);
                enemy.Gun.CurrentAmmo = enemy.Gun.MaxAmmo;
                enemy.Gun.ReloadSoundBuffer = new("Assets/Sounds/gun_reload.ogg");
                enemy.Gun.ReloadSound = new Sound(enemy.Gun.ReloadSoundBuffer);
                enemy.Gun.ShootSounds = new List<Sound>();

                enemy.Gun.Bullets = new List<BulletModel>();
                enemy.RewardXP = new Random().Next(2, 11);
                enemy.EnemyType = Model.Game.Enums.EnemyType.Basic;
                //gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);

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

        public void SpawnEnemies()
        {
            // ENEMY COLLISION DETECTION WITH WALL!!!
            foreach (var enemy in gameModel.Enemies)
            {
                for (int y = -2; y < 2; y++)
                {
                    for (int x = -2; x < 2; x++)
                    {
                        var xTilePosition = enemy.Position.X;
                        var yTilePosition = enemy.Position.Y;
                        var tilePosition = new Vector2i((int)((int)xTilePosition / gameModel.Map.TileSize.X), (int)((int)yTilePosition / gameModel.Map.TileSize.Y)) + new Vector2i(x, y);
                        var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                        if (gameModel.Map.CollidableIDs.Contains(currentTileID) == false)
                        {
                            continue;
                        }

                        var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                        var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.Map.TileSize.X, gameModel.Map.TileSize.Y);
                        var rect = enemy.GetGlobalBounds();

                        if (tileRect.Intersects(rect))
                        {
                            gameModel.Enemies.Remove(enemy);
                            enemy.IsShooting = true;
                            
                            var optimalPosition = new Vector2f();
                            var optimalDistance = float.MaxValue;

                            for (int xP = 0; xP < gameModel.Map.Size.X; xP++)
                            {
                                for (int yP = 0; yP < gameModel.Map.Size.Y; yP++)
                                {
                                    var tileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, xP, yP);
                                    if (gameModel.Map.CollidableIDs.Contains(tileID) == false)
                                    {
                                        var tileWorldPosition = tilemapLogic.GetTileWorldPosition(xP, yP);
                                        var distance = Vector2.Distance(new Vector2(rect.Left, rect.Top), new Vector2(tileWorldPosition.X, tileWorldPosition.Y));
                                        if (distance < optimalDistance)
                                        {
                                            optimalDistance = distance;
                                            optimalPosition = tileWorldPosition;
                                        }
                                    }
                                }
                            }

                            enemy.Position = optimalPosition;
                            gameModel.Enemies.Add(enemy);

                            return;
                        }
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
    }
}
