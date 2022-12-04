using Logic.UI.Interfaces;
using Model.Game.Enums;
using Model.UI.Interfaces;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.UI.Classes
{
    public class MenuUILogic : IMenuUILogic
    {
        private IMenuUIModel menuUIModel;
        private const int MAIN_MENU_MAX_NUMBER_OF_ITEMS = 4;
        private const int PAUSE_MENU_MAX_NUMBER_OF_ITEMS = 3;
        private int selectedMainMenuItemIndex;
        private int selectedPauseMenuItemIndex;
        private Color selectionColor = new Color(255, 196, 96);

        public MenuUILogic(IMenuUIModel menuUIModel)
        {
            this.menuUIModel = menuUIModel;

            menuUIModel.MainMenuTexts = new List<Text>();
            menuUIModel.PauseMenuTexts = new List<Text>();
            menuUIModel.ArrowKeysSprite = new Sprite();
            menuUIModel.GameNameText = new Text();

            var newGameText = new Text { FillColor = selectionColor, DisplayedString = "New game", Position = new Vector2f(0, 0), CharacterSize = 50 };
            var loadText = new Text { FillColor = Color.White, DisplayedString = "Load game", Position = new Vector2f(0, 0), CharacterSize = 50 };
            var highscoreText = new Text { FillColor = Color.White, DisplayedString = "Highscores", Position = new Vector2f(0, 0), CharacterSize = 50 };
            var quitText = new Text { FillColor = Color.White, DisplayedString = "Quit", Position = new Vector2f(0, 0), CharacterSize = 50 };
            menuUIModel.MainMenuTexts.Add(newGameText);
            menuUIModel.MainMenuTexts.Add(loadText);
            menuUIModel.MainMenuTexts.Add(highscoreText);
            menuUIModel.MainMenuTexts.Add(quitText);

            var resumeGameText = new Text { FillColor = selectionColor, DisplayedString = "Resume game", Position = new Vector2f(0, 0), CharacterSize = 50 };
            var mainMenuText = new Text { FillColor = Color.White, DisplayedString = "Back to main menu", Position = new Vector2f(0, 0), CharacterSize = 50 };
            menuUIModel.PauseMenuTexts.Add(resumeGameText);
            menuUIModel.PauseMenuTexts.Add(mainMenuText);
            menuUIModel.PauseMenuTexts.Add(quitText);

            // Place game name text in the top center of the screen
            var gameNameText = new Text { FillColor = Color.White, DisplayedString = "Gunner", Position = new Vector2f(0, 0), CharacterSize = 130 };
            menuUIModel.GameNameText = gameNameText;

            selectedMainMenuItemIndex = 0;
            selectedPauseMenuItemIndex = 0;
            menuUIModel.SelectedMenuOptionState = MenuOptionsState.InMainMenu;

            menuUIModel.SelectSoundBuffer = new SoundBuffer("Assets/Sounds/menu_select.ogg");
            menuUIModel.SelectSound = new Sound(menuUIModel.SelectSoundBuffer);

            menuUIModel.ConfirmSoundBuffer = new SoundBuffer("Assets/Sounds/menu_confirm.ogg");
            menuUIModel.ConfirmSound = new Sound(menuUIModel.ConfirmSoundBuffer);

            menuUIModel.PauseSoundBuffer = new SoundBuffer("Assets/Sounds/menu_pause.ogg");
            menuUIModel.PauseSound = new Sound(menuUIModel.PauseSoundBuffer);
        }

        #region Main menu
        public void UpdateMainMenu(RenderWindow window)
        {
            // Place game name text in the top center of the screen
            menuUIModel.GameNameText.Position = new Vector2f(window.Size.X / 2 - menuUIModel.GameNameText.GetLocalBounds().Width / 2, 50);
            var mouse = Mouse.GetPosition(window);

            for (int i = 0; i < menuUIModel.MainMenuTexts.Count; i++)
            {
                // Center horizontally and vertically
                menuUIModel.MainMenuTexts[i].Position = new Vector2f(window.Size.X / 2 - menuUIModel.MainMenuTexts[i].GetLocalBounds().Width / 2, menuUIModel.GameNameText.Position.Y + window.Size.Y / (MAIN_MENU_MAX_NUMBER_OF_ITEMS + 1) * (i + 1));
            }

            // Place arrow keys sprite in the bottom left of the screen
            menuUIModel.ArrowKeysSprite.Position = new Vector2f(50, window.Size.Y - (menuUIModel.ArrowKeysSprite.GetLocalBounds().Height + 50));

            //for (int i = 0; i < menuUIModel.MainMenuTexts.Count; i++)
            //{
            //    float mouseX = mouse.X;
            //    float mouseY = mouse.Y;
            //    float textX = menuUIModel.MainMenuTexts[i].Position.X;
            //    float textY = menuUIModel.MainMenuTexts[i].Position.Y;
            //    float textXPosWidth = menuUIModel.MainMenuTexts[i].Position.X + menuUIModel.MainMenuTexts[i].GetLocalBounds().Width;
            //    float textYPosHeight = menuUIModel.MainMenuTexts[i].Position.Y + menuUIModel.MainMenuTexts[i].GetLocalBounds().Height;

            //    // Check if mouse is hovering over the text
            //    if (mouseX < textXPosWidth && mouseX > textX && mouseY < textYPosHeight && mouseY > textY)
            //    {
            //        // Change color of the text
            //        menuUIModel.MainMenuTexts[i].FillColor = selectionColor;

            //        // Check if mouse is clicked
            //        if (Mouse.IsButtonPressed(Mouse.Button.Left))
            //        {
            //            selectedMainMenuItemIndex = i;
            //        }
            //    }
            //    else
            //    {
            //        // Change color of the text
            //        menuUIModel.MainMenuTexts[i].FillColor = Color.White;
            //    }
            //}
        }

        public void MoveUpMainMenu()
        {
            menuUIModel.SelectSound.Volume = 30;
            menuUIModel.SelectSound.Play();

            // Move up
            if (selectedMainMenuItemIndex - 1 >= 0)
            {
                menuUIModel.MainMenuTexts[selectedMainMenuItemIndex].FillColor = Color.White;
                selectedMainMenuItemIndex--;
                menuUIModel.MainMenuTexts[selectedMainMenuItemIndex].FillColor = selectionColor;
            }
        }

        public void MoveDownMainMenu()
        {
            menuUIModel.SelectSound.Volume = 30;
            menuUIModel.SelectSound.Play();

            // Move down
            if (selectedMainMenuItemIndex + 1 < MAIN_MENU_MAX_NUMBER_OF_ITEMS)
            {
                menuUIModel.MainMenuTexts[selectedMainMenuItemIndex].FillColor = Color.White;
                selectedMainMenuItemIndex++;
                menuUIModel.MainMenuTexts[selectedMainMenuItemIndex].FillColor = selectionColor;
            }
        }

        public MenuOptionsState GetSelectedMainMenuOption()
        {
            menuUIModel.ConfirmSound.Volume = 30;
            menuUIModel.ConfirmSound.Play();

            if (selectedMainMenuItemIndex == 0)
            {
                return MenuOptionsState.NewGame;
            }
            else if (selectedMainMenuItemIndex == 1)
            {
                return MenuOptionsState.LoadGame;
            }
            else if (selectedMainMenuItemIndex == 2)
            {
                return MenuOptionsState.Highscore;
            }
            else
            {
                return MenuOptionsState.QuitGame;
            }
        }
        #endregion

        #region Pause menu
        public void UpdatePauseMenu(Vector2u windowSize)
        {
            // Place game name text in the top center of the screen
            menuUIModel.GameNameText.Position = new Vector2f(windowSize.X / 2 - menuUIModel.GameNameText.GetLocalBounds().Width / 2, 50);

            for (int i = 0; i < menuUIModel.PauseMenuTexts.Count; i++)
            {
                // Center horizontally and vertically
                menuUIModel.PauseMenuTexts[i].Position = new Vector2f(windowSize.X / 2 - menuUIModel.PauseMenuTexts[i].GetLocalBounds().Width / 2, menuUIModel.GameNameText.Position.Y + windowSize.Y / (PAUSE_MENU_MAX_NUMBER_OF_ITEMS + 1) * (i + 1));
            }

            // Place arrow keys sprite in the bottom left of the screen
            menuUIModel.ArrowKeysSprite.Position = new Vector2f(50, windowSize.Y - (menuUIModel.ArrowKeysSprite.GetLocalBounds().Height + 50));
        }

        public void MoveUpPauseMenu()
        {
            menuUIModel.SelectSound.Volume = 30;
            menuUIModel.SelectSound.Play();
            
            // Move up
            if (selectedPauseMenuItemIndex - 1 >= 0)
            {
                menuUIModel.PauseMenuTexts[selectedPauseMenuItemIndex].FillColor = Color.White;
                selectedPauseMenuItemIndex--;
                menuUIModel.PauseMenuTexts[selectedPauseMenuItemIndex].FillColor = selectionColor;
            }
        }

        public void MoveDownPauseMenu()
        {
            menuUIModel.SelectSound.Volume = 30;
            menuUIModel.SelectSound.Play();

            // Move down
            if (selectedPauseMenuItemIndex + 1 < PAUSE_MENU_MAX_NUMBER_OF_ITEMS)
            {
                menuUIModel.PauseMenuTexts[selectedPauseMenuItemIndex].FillColor = Color.White;
                selectedPauseMenuItemIndex++;
                menuUIModel.PauseMenuTexts[selectedPauseMenuItemIndex].FillColor = selectionColor;
            }
        }

        public MenuOptionsState GetSelectedPauseMenuOption()
        {
            menuUIModel.ConfirmSound.Volume = 30;
            menuUIModel.ConfirmSound.Play();

            if (selectedPauseMenuItemIndex == 0)
            {
                return MenuOptionsState.InGame;
            }
            else if (selectedPauseMenuItemIndex == 1)
            {
                return MenuOptionsState.InMainMenu;
            }
            else
            {
                return MenuOptionsState.QuitGame;
            }
        }

        public void PlayPauseSound()
        {
            if (menuUIModel.PauseSound.Status == SoundStatus.Stopped)
            {
                menuUIModel.PauseSound.Volume = 30;
                menuUIModel.PauseSound.Play();
            }
        }
        #endregion
    }
}
