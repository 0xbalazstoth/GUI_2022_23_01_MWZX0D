using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game
{
    public enum MovementDirection
    {
        NoneOrUnknown,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        Idle
    }

    public class Movement
    {
        public MovementDirection MovementDirection { get; set; }
        public Vector2f Direction { get; set; }
    }
}
