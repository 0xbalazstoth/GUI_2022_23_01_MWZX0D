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
        private const int MAX_NUMBER_OF_ITEMS = 3;
        private int selectedItemIndex;
        private Color selectionColor = new Color(255, 196, 96);

        public MenuUILogic(IMenuUIModel menuUIModel)
        {
            this.menuUIModel = menuUIModel;

            menuUIModel.MenuTexts = new List<Text>();
            menuUIModel.ArrowKeysSprite = new Sprite();
            menuUIModel.GameNameText = new Text();

            var newGameText = new Text { FillColor = selectionColor, DisplayedString = "New game", Position = new Vector2f(0, 0), CharacterSize = 50 };
            var loadText = new Text { FillColor = Color.White, DisplayedString = "Load game", Position = new Vector2f(0, 0), CharacterSize = 50 };
            var quitText = new Text { FillColor = Color.White, DisplayedString = "Quit", Position = new Vector2f(0, 0), CharacterSize = 50 };

            menuUIModel.MenuTexts.Add(newGameText);
            menuUIModel.MenuTexts.Add(loadText);
            menuUIModel.MenuTexts.Add(quitText);

            // Place game name text in the top center of the screen
            var gameNameText = new Text { FillColor = Color.White, DisplayedString = "Gunner", Position = new Vector2f(0, 0), CharacterSize = 90 };
            menuUIModel.GameNameText = gameNameText;

            selectedItemIndex = 0;
            menuUIModel.SelectedMenuOption = MenuOptions.InMenu;
        }

        public void UpdateMenu(Vector2u windowSize)
        {
            for (int i = 0; i < menuUIModel.MenuTexts.Count; i++)
            {
                // Center horizontally and vertically
                menuUIModel.MenuTexts[i].Position = new Vector2f(windowSize.X / 2 - menuUIModel.MenuTexts[i].GetLocalBounds().Width / 2, windowSize.Y / (MAX_NUMBER_OF_ITEMS + 1) * (i + 1));
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
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = Color.White;
                selectedItemIndex--;
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = selectionColor;
            }
        }

        public void MoveDown()
        {
            // Move down
            if (selectedItemIndex + 1 < MAX_NUMBER_OF_ITEMS)
            {
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = Color.White;
                selectedItemIndex++;
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = selectionColor;
            }
        }

        public MenuOptions GetSelectedOption()
        {
            if (selectedItemIndex == 0)
            {
                return MenuOptions.NewGame;
            }
            else if (selectedItemIndex == 1)
            {
                return MenuOptions.LoadGame;
            }
            else
            {
                return MenuOptions.QuitGame;
            }
        }
    }
}
