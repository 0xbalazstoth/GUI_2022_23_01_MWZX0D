using Model.Game.Enums;
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
        public void UpdateMenu(Vector2u windowSize);
        public void MoveUp();
        public void MoveDown();
        public MenuOptions GetSelectedOption();
    }
}
