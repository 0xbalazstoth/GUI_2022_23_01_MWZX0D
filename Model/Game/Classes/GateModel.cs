using Model.Game.Enums;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class GateModel
    {
        public Sprite GateSprite { get; set; }
        public List<AnimationModel> Animations { get; set; }
        public RectangleShape Hitbox { get; set; }
        public GateState GateState { get; set; }
        public RectangleShape InteractArea { get; set; }
    }
}
