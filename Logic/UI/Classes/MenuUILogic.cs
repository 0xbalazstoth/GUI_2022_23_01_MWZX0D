using Logic.UI.Interfaces;
using Model.Game.Enums;
using Model.UI.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
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
        }

        #region Main menu
        public void UpdateMainMenu(Vector2u windowSize)
        {
            for (int i = 0; i < menuUIModel.MainMenuTexts.Count; i++)
            {
                // Center horizontally and vertically
                menuUIModel.MainMenuTexts[i].Position = new Vector2f(windowSize.X / 2 - menuUIModel.MainMenuTexts[i].GetLocalBounds().Width / 2, windowSize.Y / (MAIN_MENU_MAX_NUMBER_OF_ITEMS + 1) * (i + 1));
            }

            // Place arrow keys sprite in the bottom left of the screen
            menuUIModel.ArrowKeysSprite.Position = new Vector2f(50, windowSize.Y - (menuUIModel.ArrowKeysSprite.GetLocalBounds().Height + 50));

            // Place game name text in the top center of the screen
            menuUIModel.GameNameText.Position = new Vector2f(windowSize.X / 2 - menuUIModel.GameNameText.GetLocalBounds().Width / 2, 50);
        }

        public void MoveUpMainMenu()
        {
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
            for (int i = 0; i < menuUIModel.PauseMenuTexts.Count; i++)
            {
                // Center horizontally and vertically
                menuUIModel.PauseMenuTexts[i].Position = new Vector2f(windowSize.X / 2 - menuUIModel.PauseMenuTexts[i].GetLocalBounds().Width / 2, windowSize.Y / (PAUSE_MENU_MAX_NUMBER_OF_ITEMS + 1) * (i + 1));
            }

            // Place arrow keys sprite in the bottom left of the screen
            menuUIModel.ArrowKeysSprite.Position = new Vector2f(50, windowSize.Y - (menuUIModel.ArrowKeysSprite.GetLocalBounds().Height + 50));

            // Place game name text in the top center of the screen
            menuUIModel.GameNameText.Position = new Vector2f(windowSize.X / 2 - menuUIModel.GameNameText.GetLocalBounds().Width / 2, 50);
        }

        public void MoveUpPauseMenu()
        {
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
        #endregion
    }
}
