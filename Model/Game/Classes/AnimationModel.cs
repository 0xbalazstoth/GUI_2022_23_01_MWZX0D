using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class AnimationModel
    {
        public int Row { get; set; }
        public int ColumnsInRow { get; set; }
        public float Counter { get; set; } = 0f;
        public uint TotalRows { get; set; }
        public uint TotalColumns { get; set; }
        public float Speed { get; set; }
        public Texture Texture { get; set; }
        public IntRect TextureRect { get; set; }
        public Sprite Sprite { get; set; }
        public Vector2i GetSpriteSize
        {
            get
            {
                return new Vector2i((int)Texture.Size.X / (int)TotalColumns, (int)Texture.Size.Y / (int)TotalRows);
            }
        }
    }
}
