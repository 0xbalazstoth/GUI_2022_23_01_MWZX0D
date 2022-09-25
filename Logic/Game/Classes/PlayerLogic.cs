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
using System.Diagnostics;
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
        private IAnimationLogic animationLogic;

        private Vector2f movementDirection;
        private Vector2f previousPosition;

        public PlayerLogic(IGameModel gameModel, ITilemapLogic tilemapLogic, IAnimationLogic animationLogic, uint windowWidth, uint windowHeight)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;
            this.animationLogic = animationLogic;

            gameModel.Player = new PlayerModel();
            this.gameModel.Player.Speed = 180f;
            this.gameModel.Player.Position = new Vector2f(windowWidth / 2f, windowHeight - 100f);
            previousPosition = this.gameModel.Player.Position;

            gameModel.Player.Gun = gameModel.Guns[0];
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

        public void UpdateAnimationTextures()
        {
            gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Idle].Texture;
            gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Idle].TextureRect;
            gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);

            var movement = GetMovementByDirection(movementDirection);

            //if (movement == MovementDirection.Up)
            //{
            //    gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Up].Texture;
            //    gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Up].TextureRect;
            //    gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);
            //}
            //else if (movement == MovementDirection.Down)
            //{
            //    gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Down].Texture;
            //    gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Down].TextureRect;
            //    gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);
            //}
            //else if (movement == MovementDirection.Left)
            //{
            //    gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Left].Texture;
            //    gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Left].TextureRect;
            //    gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);
            //}
            //else if (movement == MovementDirection.Right)
            //{
            //    gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Right].Texture;
            //    gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Right].TextureRect;
            //    gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);
            //}

            if ((previousPosition.X != gameModel.Player.Position.X && previousPosition.Y != gameModel.Player.Position.Y))
            {
                gameModel.Player.Texture = gameModel.Player.Animations[movement].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[movement].TextureRect;
                gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);
            }
        }

        public void UpdateDeltaTime(float dt)
        {
            gameModel.Player.DeltaTime = dt;
        }

        public void UpdateWorldPositionByMouse(RenderWindow window)
        {
            gameModel.Player.Center = new Vector2f(gameModel.Player.Position.X + gameModel.Player.GetGlobalBounds().Width / 2f, gameModel.Player.Position.Y + gameModel.Player.GetGlobalBounds().Height / 2f);
            gameModel.MousePositionWindow = (Vector2f)Mouse.GetPosition(window);
            gameModel.WorldPositionInCamera = window.MapPixelToCoords(new Vector2i((int)gameModel.MousePositionWindow.X, (int)gameModel.MousePositionWindow.Y), gameModel.CameraView);

            gameModel.Player.AimDirection = gameModel.WorldPositionInCamera - gameModel.Player.Center;
            gameModel.Player.AimDirectionNormalized = gameModel.Player.AimDirection / (float)Math.Sqrt(gameModel.Player.AimDirection.X * gameModel.Player.AimDirection.X + gameModel.Player.AimDirection.Y * gameModel.Player.AimDirection.Y);
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

        public void FlipAndRotateGun()
        {
            var angle = (float)Math.Atan2(gameModel.Player.AimDirectionNormalized.Y, gameModel.Player.AimDirectionNormalized.X) * 180f / (float)Math.PI;

            // Change player animation texture by aim direction
            if (angle > 45f && angle < 135f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Down].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Down].TextureRect;
            }
            else if (angle < -45f && angle > -135f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Up].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Up].TextureRect;
            }
            else if (angle > 135f || angle < -135f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Left].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Left].TextureRect;
            }
            else if (angle < 45f && angle > -45f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Right].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Right].TextureRect;
            }

            // Rotate gun
            if (angle > 90f || angle < -90f)
            {
                gameModel.Player.Gun.Scale = new Vector2f(-2.5f, 2.5f);
                gameModel.Player.Gun.Rotation = angle + 180f;
                gameModel.Player.Gun.Position = new Vector2f(gameModel.Player.Position.X - 10, gameModel.Player.Position.Y + 5);
            }
            else
            {
                gameModel.Player.Gun.Scale = new Vector2f(2.5f, 2.5f);
                gameModel.Player.Gun.Rotation = angle;
                gameModel.Player.Gun.Position = new Vector2f(gameModel.Player.Position.X + 10, gameModel.Player.Position.Y + 5);
            }
        }

        public void HandleMapCollision(TilemapModel tilemap)
        {
            if (gameModel.Player.TilePosition.X < 1 || gameModel.Player.TilePosition.X > tilemap.Size.X - 1 || gameModel.Player.TilePosition.Y < 1 || gameModel.Player.TilePosition.Y > tilemap.Size.Y - 1)
            {
                gameModel.Player.Position = previousPosition;
                return;
            }

            // Change -2,2 if the player is bigger than 32x32
            
            for (int y = -2; y < 2; y++)
            {
                for (int x = -2; x < 2; x++)
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
