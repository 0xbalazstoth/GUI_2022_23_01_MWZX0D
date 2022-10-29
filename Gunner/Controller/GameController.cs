using Logic.Game.Interfaces;
using Model.Game;
using Model.Game.Classes;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static SFML.Window.Keyboard;
using Key = SFML.Window.Keyboard.Key;
using Mouse = SFML.Window.Mouse;

namespace Gunner.Controller
{
    public class GameController
    {
        private IGameModel gameModel;
        private IPlayerLogic playerLogic;

        public GameController(IGameModel gameModel, IPlayerLogic playerLogic)
        {
            this.gameModel = gameModel;
            this.playerLogic = playerLogic;
        }

        public void HandleMovementInput()
        {
            Dictionary<Key, Vector2f> input = new()
            {
               { Key.W, gameModel.MovementDirections[MovementDirection.Up].Direction },
               { Key.S, gameModel.MovementDirections[MovementDirection.Down].Direction },
               { Key.A, gameModel.MovementDirections[MovementDirection.Left].Direction },
               { Key.D, gameModel.MovementDirections[MovementDirection.Right].Direction },
            };

            Vector2f direction = new();
            foreach (var kvp in input)
            {
                if (IsKeyPressed(kvp.Key))
                {
                    direction += kvp.Value;
                }
            }

            playerLogic.HandleMovement(direction);
        }

        public void HandleShootInput()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                playerLogic.Shoot();
            }
        }

        public void HandleReloadInput()
        {
            if (IsKeyPressed(Key.R))
            {
                playerLogic.ReloadGun();
            }
        }

        public void HandleInventoryInput(KeyEventArgs eventKey)
        {
            if (eventKey.Key == System.Windows.Input.Key.I)
            {
                InventoryWindow inventoryWindow = new InventoryWindow(gameModel, playerLogic);
                inventoryWindow.ShowDialog();
            }
        }

        public void HandleDebugMode()
        {
            if (IsKeyPressed(Key.F1))
            {
                gameModel.DebugMode = true;
            }
            else if (IsKeyPressed(Key.F2))
            {
                gameModel.DebugMode = false;
            }
        }

        public void HandlePauseMenuInput(KeyEventArgs eventKey)
        {
            if (eventKey.Key == System.Windows.Input.Key.Escape)
            {
                PauseMenu pauseMenu = new PauseMenu();
                pauseMenu.Show();
            }
        }
    }
}
