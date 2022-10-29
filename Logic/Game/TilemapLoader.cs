using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Logic.Game
{
    public class TilemapLoader
    {
        private readonly Texture tilesetTexture;
        private List<Vertex[]> vertices;
        private List<int[]> mapLayers;
        private uint width, height, tileWidth, tileHeight;

        public Texture TilesetTexture => tilesetTexture;
        public List<Vertex[]> Vertices { get => vertices; }
        public List<int[]> MapLayers { get => mapLayers; }
        public uint Width { get => width; }
        public uint Height { get => height; }
        public uint TileWidth { get => tileWidth; }
        public uint TileHeight { get => tileHeight; }

        public TilemapLoader(string tilesetFile)
        {
            tilesetTexture = new Texture(tilesetFile);
        }

        public void LoadTMXFile(string tmxFile)
        {
            XDocument xdoc = XDocument.Load(tmxFile);
            var map = xdoc.Descendants("map");
            var layers = map.Descendants("layer");
            width = uint.Parse(xdoc.Element("map").Attribute("width").Value);
            height = uint.Parse(xdoc.Element("map").Attribute("height").Value);
            tileWidth = uint.Parse(xdoc.Element("map").Attribute("tilewidth").Value);
            tileHeight = uint.Parse(xdoc.Element("map").Attribute("tileheight").Value);

            mapLayers = new();
            foreach (var layer in layers)
            {
                var layerData = layer.Element("data");
                var layerValue = layerData.Value.Trim();
                var level = layerValue.Split(',').Select(x => int.Parse(x) - 1).ToArray();
                mapLayers.Add(level);
            }
        }
        
        public void InitializeVertices()
        {
            vertices = new();
            int corners = 4;

            for (int i = 0; i < mapLayers.Count; i++)
            {
                var currentVertices = new Vertex[width * height * corners * mapLayers.Count];
                for (int y = 0; y < height * corners; y += corners)
                    for (int x = 0; x < width * corners; x += corners)
                    {
                        var tileID = mapLayers[i][y / corners * width + x / corners];
                        var texCoords = GetTextureCoordinatesByTileID(tileID);
                        var tx = x / 4 * tileWidth;
                        var ty = y / 4 * tileHeight;
                        var index = y * (int)width + x;

                        currentVertices[index + 0] = new(new(tx, ty), Color.White, texCoords);
                        currentVertices[index + 1] = new(new(tx + tileWidth, ty), Color.White, new(texCoords.X + tileWidth, texCoords.Y));
                        currentVertices[index + 2] = new(new(tx + tileWidth, ty + tileHeight), Color.White, new(texCoords.X + tileHeight, texCoords.Y + tileHeight));
                        currentVertices[index + 3] = new(new(tx, ty + tileHeight), Color.White, new(texCoords.X, texCoords.Y + tileHeight));
                    }
                this.vertices.Add(currentVertices);
            }
        }
        
        public Vector2f GetTextureCoordinatesByTileID(int id)
        {
            var tilesetWidth = tilesetTexture.Size.X / tileWidth;
            var tilesetHeight = tilesetTexture.Size.Y / tileHeight;
            var texCoordsX = id % tilesetWidth;
            var texCoordsY = id / tilesetWidth;
            return new Vector2f(texCoordsX * tileWidth, texCoordsY * tileHeight);
        }
    }
}
