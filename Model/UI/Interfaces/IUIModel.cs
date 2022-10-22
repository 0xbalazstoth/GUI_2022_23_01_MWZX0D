using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UI.Interfaces
{
    public interface IUIModel
    {
        public Text FPSText { get; set; }
        public Text PlayerAmmoText { get; set; }
        public Text PlayerXPLevelText { get; set; }
        public Font Font { get; set; }
        public Sprite PlayerCoinSprite { get; set; }
        public Text PlayerCoinText { get; set; }
        public Text PlayerSpeedTimerText { get; set; }
        public Sprite PlayerSpeedSprite { get; set; }
    }
}
