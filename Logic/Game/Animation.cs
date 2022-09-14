using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game
{
    public class Animation
    {
        private Texture texture;
        private IntRect textureRect;
        private Sprite sprite;
        private float counter = 0f;
        private Vector2i spriteSize;
        private float speed = 2f;
        private int row = 0;
        private uint totalRows;
        private uint totalColumns;

        public Texture Texture { get => texture; set => texture = value; }
        public Vector2i GetSpriteSize { get => spriteSize; }
        public IntRect TextureRect { get => textureRect; set => textureRect = value; }
        public Sprite Sprite { get => sprite; set => sprite = value; }
        public int Row { get => row; set => row = value; }
        public float Speed { get => speed; set => speed = value; }

        public void Load(string filename, uint rows, uint columns)
        {
            texture = new Texture(filename);
            sprite = new Sprite(texture);

            totalRows = rows;
            totalColumns = columns;

            var spriteHeight = (int)texture.Size.Y / (int)rows;
            var spriteWidth = (int)texture.Size.X / (int)columns;

            textureRect = new IntRect(0, 0, spriteWidth, spriteHeight);

            spriteSize = new Vector2i(spriteWidth, spriteHeight);
        }

        public void Update(float dt, int columnsInRow)
        {
            counter += speed * dt;

            if (counter >= (float)columnsInRow)
            {
                counter = 0f;
            }

            sprite.TextureRect = new IntRect((int)counter * spriteSize.X, spriteSize.Y * row, spriteSize.Y, spriteSize.X);
            textureRect = sprite.TextureRect;
        }
    }
}
