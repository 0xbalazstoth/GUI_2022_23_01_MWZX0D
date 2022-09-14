using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Entities
{
    public abstract class WorldEntity : Sprite
    {
        public abstract void LoadTexture(string filename);
        public abstract void LoadTexture(Texture texture);
    }
}
