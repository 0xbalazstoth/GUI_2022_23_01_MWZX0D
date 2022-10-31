using Logic.Game.Interfaces;
using Logic.UI.Interfaces;
using Model.Game;
using Model.Game.Classes;
using Model.UI.Interfaces;
using Repository.Classes;
using Repository.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class GameControl
    {
        private IGameModel gameModel;
        private IPlayerLogic playerLogic;
        private IMenuUILogic menuUILogic;
        private IMenuUIModel menuUIModel;
        private ISaveHandler saveHandler;

        public GameControl(IGameModel gameModel, IPlayerLogic playerLogic, IMenuUILogic menuUILogic, IMenuUIModel menuUIModel)
        {
            this.gameModel = gameModel;
            this.playerLogic = playerLogic;
            this.menuUILogic = menuUILogic;
            this.menuUIModel = menuUIModel;

            this.saveHandler = new SaveHandler();
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
                gameModel.Player.IsFocusedInGame = !gameModel.Player.IsFocusedInGame;
                if (!gameModel.Player.IsFocusedInGame)
                {
                    menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InMenu;
                }
                else
                {
                    menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InGame;
                }

                saveHandler.Save(gameModel.Player.Name, gameModel);
            }
        }

        public void HandleMainMenuInput(KeyEventArgs eventKey, ref RenderWindow window)
        {
            if (eventKey.Key == System.Windows.Input.Key.Up)
            {
                menuUILogic.MoveUp();
            }

            if (eventKey.Key == System.Windows.Input.Key.Down)
            {
                menuUILogic.MoveDown();
            }

            if (eventKey.Key == System.Windows.Input.Key.Enter)
            {
                var selectedMenu = menuUILogic.GetSelectedOption();
                menuUIModel.SelectedMenuOptionState = selectedMenu;

                if (menuUIModel.SelectedMenuOptionState == Model.Game.Enums.MenuOptionsState.NewGame)
                {
                    NewGameWindow newGameWindow = new NewGameWindow();
                    newGameWindow.ShowDialog();

                    if (newGameWindow.DialogResult == true)
                    {
                        menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InGame;
                        gameModel.Player.Name = newGameWindow.PlayerName;
                    }
                }
                else if (menuUIModel.SelectedMenuOptionState == Model.Game.Enums.MenuOptionsState.LoadGame)
                {
                    var saves = saveHandler.LoadSaves();
                    LoadSavedGameWindow loadSavedGameWindow = new LoadSavedGameWindow(saves);
                    loadSavedGameWindow.ShowDialog();

                    var selectedSave = loadSavedGameWindow.SelectedSave;

                    if (loadSavedGameWindow.DialogResult == true)
                    {
                        var loadedSave = saveHandler.LoadSave(selectedSave);
                        gameModel.Player.Inventory.Capacity = loadedSave.Player.Inventory.Capacity;
                        gameModel.Player.Inventory.Items = loadedSave.Player.Inventory.Items;
                        gameModel.Player.CurrentCoins = loadedSave.Player.CurrentCoins;
                        gameModel.Player.CurrentXP = loadedSave.Player.CurrentXP;
                        gameModel.Player.Name = loadedSave.Player.Name;

                        menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InGame;
                    }
                }
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
