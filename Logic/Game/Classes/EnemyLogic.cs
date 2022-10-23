using Logic.Game.Interfaces;
using Model.Game.Classes;
using Model.Game.Enums;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Classes
{
    public class EnemyLogic : IEnemyLogic
    {
        private IGameModel gameModel;

        public EnemyLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;

            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].HPSprite = new Sprite();
                gameModel.Enemies[i].HPSprite.Position = new Vector2f(gameModel.Enemies[i].Position.X, gameModel.Enemies[i].Position.Y);

                gameModel.Enemies[i].CurrentHP = gameModel.Enemies[i].MaxHP;

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

                gameModel.Enemies[enemyIdx].Gun.LastFired = DateTime.Now;
                gameModel.Enemies[enemyIdx].Gun.Bullets.Add(tempBullet);
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
    }
}
