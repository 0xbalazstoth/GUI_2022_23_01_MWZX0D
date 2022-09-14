using Model.Game;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;

namespace Logic.Game.Entities
{
    public abstract class UnitEntity : WorldEntity
    {
        private Vector2i tilePosition;
        private Dictionary<MovementDirection, Movement> movementDirections;

        public float Speed { get; set; }
        public float DeltaTime { get; set; }
        public Vector2i TilePosition
        {
            get => tilePosition;
            private set => tilePosition = value;
        }
        public Dictionary<MovementDirection, Movement> GetMovementDirections { get => movementDirections; }

        public UnitEntity()
        {
            movementDirections = new Dictionary<MovementDirection, Movement>();
            movementDirections.Add(MovementDirection.NoneOrUnknown, new Movement() { MovementDirection = MovementDirection.NoneOrUnknown, Direction = new Vector2f(0, 0) });
            movementDirections.Add(MovementDirection.Up, new Movement() { MovementDirection = MovementDirection.Up, Direction = new Vector2f(0, -1f) });
            movementDirections.Add(MovementDirection.Down, new Movement() { MovementDirection = MovementDirection.Down, Direction = new Vector2f(0, 1f) });
            movementDirections.Add(MovementDirection.Left, new Movement() { MovementDirection = MovementDirection.Left, Direction = new Vector2f(-1f, 0) });
            movementDirections.Add(MovementDirection.Right, new Movement() { MovementDirection = MovementDirection.Right, Direction = new Vector2f(1f, 0) });
            movementDirections.Add(MovementDirection.UpLeft, new Movement() { MovementDirection = MovementDirection.UpLeft, Direction = new Vector2f(-1f, -1f) });
            movementDirections.Add(MovementDirection.UpRight, new Movement() { MovementDirection = MovementDirection.UpRight, Direction = new Vector2f(1f, -1f) });
            movementDirections.Add(MovementDirection.DownLeft, new Movement() { MovementDirection = MovementDirection.DownLeft, Direction = new Vector2f(-1f, 1f) });
            movementDirections.Add(MovementDirection.DownRight, new Movement() { MovementDirection = MovementDirection.DownRight, Direction = new Vector2f(1f, 1f) });
        }

        public override void LoadTexture(string filename)
        {
            Texture = new Texture(filename);
            Origin = new(Texture.Size.X / 2f, Texture.Size.Y / 2f);
        }

        public void UpdateTilePosition(Map tilemap)
        {
            var x = Position.X + Origin.X;
            var y = Position.Y + Origin.Y;

            TilePosition = new((int)(x / tilemap.TileSize.X), (int)(y / tilemap.TileSize.Y));
        }

        public MovementDirection GetMovementByDirection(Vector2f movementDirection)
        {
            return GetMovementDirections.Where(x => x.Value.Direction == movementDirection).FirstOrDefault().Key;
        }

        protected Vector2f GetDirectionFromInput()
        {
            Dictionary<Key, Vector2f> input = new()
            {
               { Key.W, movementDirections[MovementDirection.Up].Direction },
               { Key.S, movementDirections[MovementDirection.Down].Direction },
               { Key.A, movementDirections[MovementDirection.Left].Direction },
               { Key.D, movementDirections[MovementDirection.Right].Direction },
            };

            Vector2f direction = new();
            foreach (var kvp in input)
            {
                if (IsKeyPressed(kvp.Key))
                    direction += kvp.Value;
            }

            Vector2 numericsVector = Vector2.Normalize(new(direction.X, direction.Y));
            return new(numericsVector.X, numericsVector.Y);
        }

        public abstract void Update(float dt);
        public abstract void RedrawTexture(float dt, Texture[] texture, IntRect[] textureRect);
        protected abstract void HandleMovement();
    }
}
