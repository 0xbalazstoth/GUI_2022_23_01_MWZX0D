using Model.Game;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Entities
{
    public class Player : UnitEntity
    {
        private Vector2f previousPosition;
        private uint mapHeight;
        private uint mapWidth;
        private Vector2f movementDirection;

        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public string Name { get; set; }

        public Player(uint mapHeight, uint mapWidth)
        {

            this.mapHeight = mapHeight;
            this.mapWidth = mapWidth;

            Speed = 180f;
        }

        public override void Update(float dt)
        {
            DeltaTime = dt;

            HandleMovement();
        }

        public override void RedrawTexture(float dt, Texture[] texture, IntRect[] textureRect)
        {
            Texture = texture[0];
            TextureRect = textureRect[0];

            var movement = GetMovementByDirection(movementDirection);
            if (movement == MovementDirection.Up)
            {
                Texture = texture[3];
                TextureRect = textureRect[3];
            }
            else if (movement == MovementDirection.Down)
            {
                Texture = texture[1];
                TextureRect = textureRect[1];
            }
            else if (movement == MovementDirection.Left)
            {
                Texture = texture[2];
                TextureRect = textureRect[2];
            }
            else if (movement == MovementDirection.Right)
            {
                Texture = texture[4];
                TextureRect = textureRect[4];
            }
        }

        public void RedrawTexture(float dt, Texture texture, IntRect textureRect)
        {
            Texture = texture;
            TextureRect = textureRect;
        }

        public void HandleItemCollision(ItemEntity item)
        {
            if (GetGlobalBounds().Intersects(item.GetGlobalBounds()))
            {
                Console.WriteLine(item.ToString());
                Position = previousPosition;
            }
        }

        public void HandleEnemyCollision(Enemy enemy)
        {
            if (GetGlobalBounds().Intersects(enemy.GetGlobalBounds()))
            {
                Console.WriteLine(enemy.ToString());
                Position = previousPosition;
            }
        }

        public void HandleMapCollision(Map tilemap)
        {
            //if (TilePosition.X < 1 || TilePosition.X > tilemap.Size.X - 1 || TilePosition.Y < 1 || TilePosition.Y > tilemap.Size.Y - 1)
            //{
            //    Console.WriteLine(tilemap.ToString());
            //    Position = previousPosition;
            //    return;
            //}

            //// Next positions
            //for (int y = -1; y < 1; y++)
            //{
            //    for (int x = -1; x < 1; x++)
            //    {
            //        var currentTilePosition = TilePosition + new Vector2i(x, y);
            //        var currentTileID = tilemap.GetTileID(TilemapLogic.COLLISION_LAYER, currentTilePosition.X, currentTilePosition.Y);
            //        if (tilemap.CollidableIDs.Contains(currentTileID) == false)
            //        {
            //            continue;
            //        }

            //        var currentTileWorldPosition = tilemap.GetTileWorldPosition(currentTilePosition.X, currentTilePosition.Y);
            //        var tileRect = new FloatRect(currentTileWorldPosition, new(tilemap.TileSize.X, tilemap.TileSize.Y));
            //        var rect = GetGlobalBounds();
            //        if (rect.Intersects(tileRect))
            //        {
            //            Console.WriteLine(tilemap.ToString());
            //            Position = previousPosition;
            //            return;
            //        }
            //    }
            //}
        }

        protected override void HandleMovement()
        {
            var movementDirection = GetDirectionFromInput();
            if (float.IsNaN(movementDirection.X) || float.IsNaN(movementDirection.Y))
                return;

            previousPosition = Position;
            Position += movementDirection * DeltaTime * Speed;
            this.movementDirection = movementDirection;
        }

        public override void LoadTexture(Texture texture)
        {
            throw new NotImplementedException();
        }
    }
}
