﻿using Logic.UI.Interfaces;
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
        private const int MAX_NUMBER_OF_ITEMS = 4;
        private int selectedItemIndex;
        private Color selectionColor = new Color(255, 196, 96);

        public MenuUILogic(IMenuUIModel menuUIModel)
        {
            this.menuUIModel = menuUIModel;

            menuUIModel.MainMenuTexts = new List<Text>();
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

            // Place game name text in the top center of the screen
            var gameNameText = new Text { FillColor = Color.White, DisplayedString = "Gunner", Position = new Vector2f(0, 0), CharacterSize = 90 };
            menuUIModel.GameNameText = gameNameText;

            selectedItemIndex = 0;
            menuUIModel.SelectedMenuOptionState = MenuOptionsState.InMenu;
        }

        public void UpdateMenu(Vector2u windowSize)
        {
            for (int i = 0; i < menuUIModel.MainMenuTexts.Count; i++)
            {
                // Center horizontally and vertically
                menuUIModel.MainMenuTexts[i].Position = new Vector2f(windowSize.X / 2 - menuUIModel.MainMenuTexts[i].GetLocalBounds().Width / 2, windowSize.Y / (MAX_NUMBER_OF_ITEMS + 1) * (i + 1));
            }

            // Place arrow keys sprite in the bottom left of the screen
            menuUIModel.ArrowKeysSprite.Position = new Vector2f(50, windowSize.Y - (menuUIModel.ArrowKeysSprite.GetLocalBounds().Height + 50));

            // Place game name text in the top center of the screen
            menuUIModel.GameNameText.Position = new Vector2f(windowSize.X / 2 - menuUIModel.GameNameText.GetLocalBounds().Width / 2, 50);
        }

        public void MoveUp()
        {
            // Move up
            if (selectedItemIndex - 1 >= 0)
            {
                menuUIModel.MainMenuTexts[selectedItemIndex].FillColor = Color.White;
                selectedItemIndex--;
                menuUIModel.MainMenuTexts[selectedItemIndex].FillColor = selectionColor;
            }
        }

        public void MoveDown()
        {
            // Move down
            if (selectedItemIndex + 1 < MAX_NUMBER_OF_ITEMS)
            {
                menuUIModel.MainMenuTexts[selectedItemIndex].FillColor = Color.White;
                selectedItemIndex++;
                menuUIModel.MainMenuTexts[selectedItemIndex].FillColor = selectionColor;
            }
        }

        public MenuOptionsState GetSelectedOption()
        {
            if (selectedItemIndex == 0)
            {
                return MenuOptionsState.NewGame;
            }
            else if (selectedItemIndex == 1)
            {
                return MenuOptionsState.LoadGame;
            }
            else if (selectedItemIndex == 2)
            {
                return MenuOptionsState.Highscore;
            }
            else
            {
                return MenuOptionsState.QuitGame;
            }
        }
    }
}
