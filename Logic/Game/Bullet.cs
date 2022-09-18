using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game
{
    public class Bullet
    {
        public CircleShape shape { get; set; }
        public Vector2f currVelocity { get; set; }
        public float maxSpeed { get; set; }

        public Bullet(float radius = 5f)
        {
            currVelocity = new Vector2f(0, 0);
            maxSpeed = 15f;

            shape = new CircleShape(radius);
            shape.FillColor = Color.Red;
        }
    }
}
