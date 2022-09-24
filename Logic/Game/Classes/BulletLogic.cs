using Logic.Game.Interfaces;
using Model.Game.Classes;
using SFML.Graphics;
using SFML.System;
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

            GunModel pistol = new GunModel();
            pistol.GunType = GunType.Pistol;
            pistol.Damage = 10;
            pistol.MaxAmmo = 15;
            pistol.Texture = new Texture("Assets/Textures/pistol.png");
            pistol.TextureRect = new IntRect(0, 0, 12, 3);
            pistol.Scale = new Vector2f(2, 2);

            gameModel.Guns = new List<GunModel>();
            gameModel.Guns.Add(pistol);
        }

        public void HandleCollision(RenderWindow window)
        {
            // Check if bullets are in the window/screen

            //for (int i = 0; i < gameModel.Bullets.Count; i++)
            //{
            //    if (gameModel.Bullets[i].Shape.Position.X < 0
            //        || gameModel.Bullets[i].Shape.Position.X > window.Size.X
            //        || gameModel.Bullets[i].Shape.Position.Y < 0
            //        || gameModel.Bullets[i].Shape.Position.Y > window.Size.Y)
            //    {
            //        gameModel.Bullets.RemoveAt(i);
            //    }
            //}
        }

        public void Shoot()
        {
            BulletModel tempBullet = new BulletModel();
            tempBullet.Shape = new CircleShape(5);
            tempBullet.Speed = 15f;
            tempBullet.Shape.Position = gameModel.Player.Gun.Position;
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

                if (Math.Sqrt(distX * distX + distY * distY) > 600)
                {
                    gameModel.Bullets.RemoveAt(i);
                }
            }
        }
    }
}
