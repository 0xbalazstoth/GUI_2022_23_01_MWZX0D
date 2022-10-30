using Logic.Game.Interfaces;
using Model.Game;
using Model.Game.Classes;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        
        public void HandleGunSwitchInput(RenderWindow window)
        {
            window.MouseWheelScrolled += (s, e) =>
            {
                int gunIdx = 0;
                gameModel.Player.Gun.Bullets = new List<BulletModel>();

                // Check if the player has more than one gun
                if (gameModel.Guns.Count > 1)
                {
                    // Check if the player is scrolling up or down
                    if (e.Delta > 0)
                    {
                        // Check if the player is on the last gun
                        if (gameModel.Guns.IndexOf(gameModel.Player.Gun) == gameModel.Guns.Count - 1)
                        {
                            gunIdx = 0;
                        }
                        else
                        {
                            gunIdx = gameModel.Guns.IndexOf(gameModel.Player.Gun) + 1;
                        }
                    }
                    else if (e.Delta < 0)
                    {
                        // Check if the player is on the first gun
                        if (gameModel.Guns.IndexOf(gameModel.Player.Gun) == 0)
                        {
                            gunIdx = gameModel.Guns.Count - 1;
                        }
                        else
                        {
                            gunIdx = gameModel.Guns.IndexOf(gameModel.Player.Gun) - 1;
                        }
                    }

                    // Set the player's gun to the new gun
                    gameModel.Player.Gun = gameModel.Guns[gunIdx];
                }
            };
        }
    }
}
