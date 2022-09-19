using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class TilemapModel
    {
        public Vector2u Size { get; set; }
        public Vector2u TileSize { get; set; }
        public List<int> CollidableIDs { get; set; }
        public string TMXFile { get; set; }
        public string TilesetFile { get; set; }
        public Texture TilesetTexture { get; set; }
        public List<Vertex[]> Vertices { get; set; }
        public List<int[]> MapLayers { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint TileWidth { get; set; }
        public uint TileHeight { get; set; }
        public uint GetMapHeight { get => Height * TileHeight; }
        public uint GetMapWidth { get => Width * TileWidth; }
    }
}