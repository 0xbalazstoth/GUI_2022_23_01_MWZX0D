using Logic.Game.Interfaces;
using Model.Game.Classes;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Classes
{
    public class BulletLogic : IBulletLogic
    {
        private IGameModel gameModel;

        public BulletLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;
            gameModel.Bullets = new List<BulletModel>();
        }

        public void Shoot()
        {
            BulletModel tempBullet = new BulletModel();
            tempBullet.Shape = new CircleShape(5);
            tempBullet.Speed = 15f;
            tempBullet.Shape.Position = gameModel.Player.Center;
            tempBullet.Velocity = gameModel.Player.AimDirectionNormalized * tempBullet.Speed;

            gameModel.Bullets.Add(tempBullet);
        }

        public void Update()
        {
            for (int i = 0; i < gameModel.Bullets.Count; i++)
            {
                gameModel.Bullets[i].Shape.Position += gameModel.Bullets[i].Velocity;

                float distX = gameModel.Bullets[i].Shape.Position.X - gameModel.Player.Center.X;
                float distY = gameModel.Bullets[i].Shape.Position.Y - gameModel.Player.Center.Y;

                if (Math.Sqrt(distX * distX + distY * distY) > 1000)
                {
                    gameModel.Bullets.RemoveAt(i);
                }
            }
        }
    }
}
