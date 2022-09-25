using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Game.Interfaces;
using Model.Game.Classes;
using Model.Game;

namespace Logic.Game.Classes
{
    public class AnimationLogic : IAnimationLogic
    {
        private IGameModel gameModel;

        public AnimationLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;

            gameModel.Player.Animations = new Dictionary<MovementDirection, AnimationModel>();
            gameModel.Player.Animations.Add(MovementDirection.Idle, new AnimationModel() {
                Row = 0,
                ColumnsInRow = 5,
                TotalRows = 1,
                TotalColumns = 5,
                Speed = 10f,
                Texture = new Texture("idle_right.png"),
            });
            gameModel.Player.Animations[MovementDirection.Idle].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Idle].Texture);
            gameModel.Player.Animations[MovementDirection.Idle].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Idle].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Idle].GetSpriteSize.Y);

            gameModel.Player.Animations.Add(MovementDirection.Left, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 4,
                TotalRows = 1,
                TotalColumns = 4,
                Speed = 10f,
                Texture = new Texture("move_left.png"),
            });
            gameModel.Player.Animations[MovementDirection.Left].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Left].Texture);
            gameModel.Player.Animations[MovementDirection.Left].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Left].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Left].GetSpriteSize.Y);

            gameModel.Player.Animations.Add(MovementDirection.Right, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 4,
                TotalRows = 1,
                TotalColumns = 4,
                Speed = 10f,
                Texture = new Texture("move_right.png"),
            });
            gameModel.Player.Animations[MovementDirection.Right].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Right].Texture);
            gameModel.Player.Animations[MovementDirection.Right].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Right].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Right].GetSpriteSize.Y);

            gameModel.Player.Animations.Add(MovementDirection.Up, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 8,
                TotalRows = 1,
                TotalColumns = 8,
                Speed = 10f,
                Texture = new Texture("move_up.png"),
            });
            gameModel.Player.Animations[MovementDirection.Up].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Up].Texture);
            gameModel.Player.Animations[MovementDirection.Up].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Up].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Up].GetSpriteSize.Y);

            gameModel.Player.Animations.Add(MovementDirection.Down, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 8,
                TotalRows = 1,
                TotalColumns = 8,
                Speed = 10f,
                Texture = new Texture("move_down.png"),
            });
            gameModel.Player.Animations[MovementDirection.Down].Sprite = new Sprite(gameModel.Player.Animations[MovementDirection.Down].Texture);
            gameModel.Player.Animations[MovementDirection.Down].TextureRect = new IntRect(0, 0, gameModel.Player.Animations[MovementDirection.Down].GetSpriteSize.X, gameModel.Player.Animations[MovementDirection.Down].GetSpriteSize.Y);
        }

        public void Update(float dt, int columnsInRow)
        {
            foreach (var playerAnimation in gameModel.Player.Animations)
            {
                playerAnimation.Value.Counter += playerAnimation.Value.Speed * dt;

                if (playerAnimation.Value.Counter >= (float)playerAnimation.Value.ColumnsInRow)
                {
                    playerAnimation.Value.Counter = 0f;
                }
                playerAnimation.Value.TextureRect = new IntRect((int)playerAnimation.Value.Counter * playerAnimation.Value.GetSpriteSize.X, playerAnimation.Value.Row * playerAnimation.Value.GetSpriteSize.Y, playerAnimation.Value.GetSpriteSize.X, playerAnimation.Value.GetSpriteSize.Y);
                gameModel.Player.Animations[playerAnimation.Key].TextureRect = playerAnimation.Value.TextureRect;
            }
        }
    }
}
