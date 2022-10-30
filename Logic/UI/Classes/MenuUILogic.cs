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

        public MenuUILogic(IMenuUIModel menuUIModel)
        {
            this.menuUIModel = menuUIModel;

            menuUIModel.MenuTexts = new List<Text>();

            var newGameText = new Text { FillColor = Color.Red, DisplayedString = "New game", Position = new Vector2f(0, 0) };
            var loadText = new Text { FillColor = Color.White, DisplayedString = "Load game", Position = new Vector2f(0, 0) };
            var quitText = new Text { FillColor = Color.White, DisplayedString = "Quit", Position = new Vector2f(0, 0) };

            menuUIModel.MenuTexts.Add(newGameText);
            menuUIModel.MenuTexts.Add(loadText);
            menuUIModel.MenuTexts.Add(quitText);

            selectedItemIndex = 0;
            menuUIModel.SelectedMenuOption = MenuOptions.Nothing;
        }

        public void UpdateMenu(Vector2u windowSize)
        {
            for (int i = 0; i < menuUIModel.MenuTexts.Count; i++)
            {
                // Center horizontally and vertically
                menuUIModel.MenuTexts[i].Position = new Vector2f(windowSize.X / 2 - menuUIModel.MenuTexts[i].GetLocalBounds().Width / 2, windowSize.Y / (MAX_NUMBER_OF_ITEMS + 1) * (i + 1));
            }
        }

        public void MoveUp()
        {
            // Move up
            if (selectedItemIndex - 1 >= 0)
            {
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = Color.White;
                selectedItemIndex--;
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = Color.Red;
            }
        }

        public void MoveDown()
        {
            // Move down
            if (selectedItemIndex + 1 < MAX_NUMBER_OF_ITEMS)
            {
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = Color.White;
                selectedItemIndex++;
                menuUIModel.MenuTexts[selectedItemIndex].FillColor = Color.Red;
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
