﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public abstract class UnitEntityModel : Sprite
    {
        public float Speed { get; set; }
        public float DeltaTime { get; set; }
        public Vector2i TilePosition { get; set; }
        public Dictionary<MovementDirection, Movement> MovementDirections { get; set; }
    }
}