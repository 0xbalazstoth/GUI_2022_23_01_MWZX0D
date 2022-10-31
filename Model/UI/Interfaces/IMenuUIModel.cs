using Model.Game.Enums;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UI.Interfaces
{
    public interface IMenuUIModel
    {
        public List<Text> MenuTexts { get; set; }
        public Font Font { get; set; }
        public MenuOptions SelectedMenuOption { get; set; }
        public Sprite ArrowKeysSprite { get; set; }
        public Text GameNameText { get; set; }
    }
}
