using Logic.Game.Interfaces;
using Model.Game.Classes;
using SFML.Graphics;
using SFML.System;
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

                //gameModel.Enemies[i].HPText = new Text();
                gameModel.Enemies[i].HPText.Position = new Vector2f(gameModel.Enemies[i].Position.X, gameModel.Enemies[i].Position.Y);
                gameModel.Enemies[i].HPText.CharacterSize = 16;
                gameModel.Enemies[i].HPText.FillColor = Color.Red;
            }
        }

        public void ChasePlayer()
        {
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
                        gameModel.Enemies.Remove(enemy);

                        gameModel.Player.CurrentXP += enemy.RewardXP;
                        break;
                    }
                }
            }
        }

        public void HandleMovement()
        {
            
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

        public void UpdateDeltaTile(float dt)
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
    }
}
