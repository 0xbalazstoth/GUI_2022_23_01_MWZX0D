﻿using Model.Game.Enums;
using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class BulletModel
    {
        public CircleShape Bullet { get; set; }
        public Vector2f Velocity { get; set; }
        public float Speed { get; set; }
        public Dictionary<GunType, AnimationModel> Animations { get; set; }
    }
}
