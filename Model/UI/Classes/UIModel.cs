using Model.UI.Interfaces;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UI.Classes
{
    public class UIModel : IUIModel
    {
        public Text FPSText { get; set; }
        public Font Font { get; set; }

        public Text AmmoText { get; set; }
    }
}
