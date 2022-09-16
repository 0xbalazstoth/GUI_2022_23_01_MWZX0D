using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Entities
{
    public abstract class ItemEntity : Sprite
    {
        private Vector2i itemPosition;

        public Vector2i Size { get; set; }
        public Vector2i ItemPosition
        {
            get => itemPosition;
            private set => itemPosition = value;
        }

        public void LoadTexture(string filename)
        {
            Texture = new Texture(filename);
            Origin = new(Texture.Size.X / 2f, Texture.Size.Y / 2f);
            Scale = new Vector2f((float)Size.X / Texture.Size.X, (float)Size.Y / Texture.Size.Y);
        }

        public void LoadTexture(Texture texture)
        {
            Texture = texture;
            Origin = new(Texture.Size.X / 2f, Texture.Size.Y / 2f);
            Scale = new Vector2f((float)Size.X / Texture.Size.X, (float)Size.Y / Texture.Size.Y);
        }

        public abstract void Update(float dt);
    }
}
