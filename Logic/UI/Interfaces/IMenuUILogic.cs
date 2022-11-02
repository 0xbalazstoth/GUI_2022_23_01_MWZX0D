using Model.Game.Enums;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.UI.Interfaces
{
    public interface IMenuUILogic
    {
        public void UpdateMainMenu(Vector2u windowSize);
        public void MoveUpMainMenu();
        public void MoveDownMainMenu();
        public MenuOptionsState GetSelectedMainMenuOption();
        public void UpdatePauseMenu(Vector2u windowSize);
        public void MoveUpPauseMenu();
        public void MoveDownPauseMenu();
        public MenuOptionsState GetSelectedPauseMenuOption();
    }
}
