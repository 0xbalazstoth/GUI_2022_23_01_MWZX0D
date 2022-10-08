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
            pistol.Texture = new Texture("Assets/Textures/pistol.png");
            pistol.TextureRect = new IntRect(0, 0, 12, 3);
            pistol.Scale = new Vector2f(2, 2);

            gameModel.Guns = new List<GunModel>();
            gameModel.Guns.Add(pistol);
        }

        public void HandleMapCollision(RenderWindow window)
        {
            foreach (BulletModel bullet in gameModel.Player.Bullets)
            {
                var xTileposition = bullet.Shape.Position.X + bullet.Shape.Origin.X;
                var yTileposition = bullet.Shape.Position.Y + bullet.Shape.Origin.Y;
                var tilePosition = new Vector2i((int)((int)xTileposition / gameModel.Map.TileSize.X), (int)((int)yTileposition / gameModel.Map.TileSize.Y));

                var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                if (gameModel.Map.CollidableIDs.Contains(currentTileID) == false)
                {
                    continue;
                }

                var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.Map.TileSize.X, gameModel.Map.TileSize.Y);
                var rect = bullet.Shape.GetGlobalBounds();

                if (rect.Intersects(tileRect))
                {
                    gameModel.Player.Bullets.Remove(bullet);
                    return;
                }
            }
        }

        public void HandleObjectCollision(Sprite item)
        {
            foreach (var bullet in gameModel.Player.Bullets)
            {
                if (bullet.Shape.GetGlobalBounds().Intersects(item.GetGlobalBounds()))
                {
                    gameModel.Player.Bullets.Remove(bullet);
                    return;
                }
            }
        }

        public void Shoot()
        {
            BulletModel tempBullet = new BulletModel();
            tempBullet.Shape = new CircleShape(5);
            tempBullet.Speed = 15f;
            tempBullet.Shape.Position = gameModel.Player.Gun.Position;
            tempBullet.Velocity = gameModel.Player.AimDirectionNormalized * tempBullet.Speed;

            gameModel.Player.Bullets.Add(tempBullet);
        }

        public void Update()
        {
            for (int i = 0; i < gameModel.Player.Bullets.Count; i++)
            {
                gameModel.Player.Bullets[i].Shape.Position += gameModel.Player.Bullets[i].Velocity;

                float distX = gameModel.Player.Bullets[i].Shape.Position.X - gameModel.Player.Center.X;
                float distY = gameModel.Player.Bullets[i].Shape.Position.Y - gameModel.Player.Center.Y;

                if (Math.Sqrt(distX * distX + distY * distY) > 600)
                {
                    gameModel.Player.Bullets.RemoveAt(i);
                }
            }
        }
    }
}
