using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game
{
    public interface IGameModel
    {
        public Movement Movement { get; set; }
        public View CameraView { get; set; }
        public View UIView { get; set; }
        public Map Map { get; set; }
    }
}
