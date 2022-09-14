using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UI
{
    public interface IUIModel
    {
        public Text FPSText { get; set; }
        public Font Font { get; set; }
    }
}
