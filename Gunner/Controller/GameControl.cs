using Logic.Game.Interfaces;
using Logic.UI.Interfaces;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Enums;
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
        private IHighscoreHandler highscoreHandler;

        public GameControl(IGameModel gameModel, IPlayerLogic playerLogic, IMenuUILogic menuUILogic, IMenuUIModel menuUIModel)
        {
            this.gameModel = gameModel;
            this.playerLogic = playerLogic;
            this.menuUILogic = menuUILogic;
            this.menuUIModel = menuUIModel;

            this.saveHandler = new SaveHandler();
            this.highscoreHandler = new HighscoreHandler();
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

        public void HandleGateInput(KeyEventArgs eventKey)
        {
            if (gameModel.Player.PlayerState == GateState.InShop)
            {
                if (eventKey.Key == System.Windows.Input.Key.E)
                {
                    ShopWindow shopWindow = new ShopWindow(gameModel, playerLogic);
                    shopWindow.ShowDialog();
                }
            }
        }

        public void HandlePauseMenuInput(KeyEventArgs eventKey)
        {
            if (eventKey.Key == System.Windows.Input.Key.Escape)
            {
                gameModel.Player.IsFocusedInGame = !gameModel.Player.IsFocusedInGame;
                if (!gameModel.Player.IsFocusedInGame)
                {
                    menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InPauseMenu;
                }
                else
                {
                    menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InGame;
                }

                saveHandler.Save(gameModel.Player.Name, gameModel);
            }

            if (menuUIModel.SelectedMenuOptionState == Model.Game.Enums.MenuOptionsState.InPauseMenu)
            {
                if (eventKey.Key == System.Windows.Input.Key.Up)
                {
                    menuUILogic.MoveUpPauseMenu();
                }

                if (eventKey.Key == System.Windows.Input.Key.Down)
                {
                    menuUILogic.MoveDownPauseMenu();
                }

                if (eventKey.Key == System.Windows.Input.Key.Enter)
                {
                    var selectedMenu = menuUILogic.GetSelectedPauseMenuOption();

                    if (selectedMenu == Model.Game.Enums.MenuOptionsState.InGame)
                    {
                        gameModel.Player.IsFocusedInGame = true;
                        menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InGame;
                    }
                    else if (selectedMenu == Model.Game.Enums.MenuOptionsState.InMainMenu)
                    {
                        // Restart app
                        Application.Current.Shutdown();
                        System.Windows.Forms.Application.Restart();
                    }
                    else if (selectedMenu == Model.Game.Enums.MenuOptionsState.QuitGame)
                    {
                        menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.QuitGame;
                    }
                }
            }
        }

        public void HandleMainMenuInput(KeyEventArgs eventKey)
        {
            if (menuUIModel.SelectedMenuOptionState == Model.Game.Enums.MenuOptionsState.InMainMenu)
            {
                if (eventKey.Key == System.Windows.Input.Key.Up)
                {
                    menuUILogic.MoveUpMainMenu();
                }

                if (eventKey.Key == System.Windows.Input.Key.Down)
                {
                    menuUILogic.MoveDownMainMenu();
                }

                if (eventKey.Key == System.Windows.Input.Key.Enter)
                {
                    var selectedMenu = menuUILogic.GetSelectedMainMenuOption();
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
                        else
                        {
                            menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InMainMenu;
                        }
                    }
                    else if (menuUIModel.SelectedMenuOptionState == Model.Game.Enums.MenuOptionsState.Highscore)
                    {
                        var highscores = highscoreHandler.LoadHighscoreStats();

                        HighscoreWindow highscoreWindow = new HighscoreWindow(highscores);
                        highscoreWindow.ShowDialog();

                        if (highscoreWindow.DialogResult == true)
                        {
                            menuUIModel.SelectedMenuOptionState = Model.Game.Enums.MenuOptionsState.InMainMenu;
                        }
                    }
                }
            }
        }

        public void HandleRespawnInput(KeyEventArgs eventKey)
        {
            if (gameModel.Player.IsDead)
            {
                if (eventKey.Key == System.Windows.Input.Key.Space)
                {
                    gameModel.Player.IsDead = false;
                    gameModel.Player.CurrentHP = 100;
                    gameModel.Player.Position = new Vector2f(300, 300);

                    gameModel.CurrentMap.Vertices = gameModel.LobbyMap.Vertices;
                    gameModel.CurrentMap.MapLayers = gameModel.LobbyMap.MapLayers;
                    gameModel.CurrentMap.Width = gameModel.LobbyMap.Width;
                    gameModel.CurrentMap.Height = gameModel.LobbyMap.Height;
                    gameModel.CurrentMap.TileWidth = gameModel.LobbyMap.TileWidth;
                    gameModel.CurrentMap.TileHeight = gameModel.LobbyMap.TileHeight;
                    gameModel.CurrentMap.Size = new Vector2u(gameModel.LobbyMap.Width, gameModel.LobbyMap.Height);
                    gameModel.CurrentMap.TileSize = new Vector2u(gameModel.LobbyMap.TileWidth, gameModel.LobbyMap.TileHeight);
                    gameModel.Player.PlayerState = GateState.InLobby;
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
