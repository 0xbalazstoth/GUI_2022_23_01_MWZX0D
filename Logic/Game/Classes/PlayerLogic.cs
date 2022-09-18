using Logic.Game.Classes;
using Logic.Game.Interfaces;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Classes
{
    public class PlayerLogic : IPlayerLogic
    {
        private IGameModel gameModel;
        private ITilemapLogic tilemapLogic;

        private Vector2f movementDirection;
        private Vector2f previousPosition;

        public PlayerLogic(IGameModel gameModel, ITilemapLogic tilemapLogic, uint windowWidth, uint windowHeight)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;

            gameModel.Player = new PlayerModel();
            this.gameModel.Player.Speed = 180f;
            this.gameModel.Player.Position = new Vector2f(windowWidth / 2f, windowHeight - 100f);
        }

        public Vector2f GetDirectionFromInput(Vector2f direction)
        {
            Vector2 numericsVector = Vector2.Normalize(new(direction.X, direction.Y));
            return new(numericsVector.X, numericsVector.Y);
        }

        public MovementDirection GetMovementByDirection(Vector2f movementDirection)
        {
            return gameModel.MovementDirections.Where(x => x.Value.Direction == movementDirection).FirstOrDefault().Key;
        }

        public void LoadTexture(string filename)
        {
            gameModel.Player.Texture = new Texture(filename);
            gameModel.Player.Origin = new Vector2f(gameModel.Player.Texture.Size.X / 2, gameModel.Player.Texture.Size.Y / 2);
        }

        public void UpdateTilePosition(TilemapModel tilemap)
        {
            var x = gameModel.Player.Position.X + gameModel.Player.Origin.X;
            var y = gameModel.Player.Position.Y + gameModel.Player.Origin.Y;

            gameModel.Player.TilePosition = new Vector2i((int)(x / tilemap.TileSize.X), (int)(y / tilemap.TileSize.Y));
        }

        public void HandleMovement(Vector2f direction)
        {
            var movementDirection = GetDirectionFromInput(direction);
            if (float.IsNaN(movementDirection.X) || float.IsNaN(movementDirection.Y))
                return;

            previousPosition = gameModel.Player.Position;
            gameModel.Player.Position += movementDirection * gameModel.Player.DeltaTime * gameModel.Player.Speed;
            this.movementDirection = movementDirection;
        }

        public void LoadTexture(Texture texture)
        {
            
        }

        public void UpdateAnimationTextures(float dt, Texture[] texture, IntRect[] textureRect)
        {
            gameModel.Player.Texture = texture[0];
            gameModel.Player.TextureRect = textureRect[0];

            var movement = GetMovementByDirection(movementDirection);
            if (movement == MovementDirection.Up)
            {
                gameModel.Player.Texture = texture[3];
                gameModel.Player.TextureRect = textureRect[3];
            }
            else if (movement == MovementDirection.Down)
            {
                gameModel.Player.Texture = texture[1];
                gameModel.Player.TextureRect = textureRect[1];
            }
            else if (movement == MovementDirection.Left)
            {
                gameModel.Player.Texture = texture[2];
                gameModel.Player.TextureRect = textureRect[2];
            }
            else if (movement == MovementDirection.Right)
            {
                gameModel.Player.Texture = texture[4];
                gameModel.Player.TextureRect = textureRect[4];
            }
        }

        public void UpdateDeltaTime(float dt)
        {
            gameModel.Player.DeltaTime = dt;
        }

        public void HandleEnemyCollision(EnemyModel enemy)
        {
            if (gameModel.Player.GetGlobalBounds().Intersects(enemy.GetGlobalBounds()))
            {
                gameModel.Player.Position = previousPosition;
            }
        }

        public void HandleObjectCollision(Sprite item)
        {
            if (gameModel.Player.GetGlobalBounds().Intersects(item.GetGlobalBounds()))
            {
                gameModel.Player.Position = previousPosition;
            }
        }

        public void HandleMapCollision(TilemapModel tilemap)
        {
            if (gameModel.Player.TilePosition.X < 1 || gameModel.Player.TilePosition.X > tilemap.Size.X - 1 || gameModel.Player.TilePosition.Y < 1 || gameModel.Player.TilePosition.Y > tilemap.Size.Y - 1)
            {
                gameModel.Player.Position = previousPosition;
                return;
            }

            // Next positions
            for (int y = -1; y < 1; y++)
            {
                for (int x = -1; x < 1; x++)
                {
                    var currentTilePosition = gameModel.Player.TilePosition + new Vector2i(x, y);
                    var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, currentTilePosition.X, currentTilePosition.Y);
                    if (tilemap.CollidableIDs.Contains(currentTileID) == false)
                    {
                        continue;
                    }

                    var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(currentTilePosition.X, currentTilePosition.Y);
                    var tileRect = new FloatRect(currentTileWorldPosition, new(tilemap.TileSize.X, tilemap.TileSize.Y));
                    var rect = gameModel.Player.GetGlobalBounds();
                    if (rect.Intersects(tileRect))
                    {
                        gameModel.Player.Position = previousPosition;
                        return;
                    }
                }
            }
        }
    }
}
