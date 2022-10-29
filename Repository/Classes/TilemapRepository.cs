using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.Game.Classes;
using Repository.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Repository.Classes
{
    public class TilemapRepository : ITilemapRepository
    {
        private List<Vertex[]> vertices;
        private List<int[]> mapLayers;
        private uint width, height, tileWidth, tileHeight;

        public TilemapRepository()
        {
            vertices = new();
            mapLayers = new();
        }

        public TilemapModel LoadTMXFile(string tmxFile)
        {
            XDocument xdoc = XDocument.Load(tmxFile);
            var map = xdoc.Descendants("map");
            var layers = map.Descendants("layer");
            width = uint.Parse(xdoc.Element("map").Attribute("width").Value);
            height = uint.Parse(xdoc.Element("map").Attribute("height").Value);
            tileWidth = uint.Parse(xdoc.Element("map").Attribute("tilewidth").Value);
            tileHeight = uint.Parse(xdoc.Element("map").Attribute("tileheight").Value);

            foreach (var layer in layers)
            {
                var layerData = layer.Element("data");
                var layerValue = layerData.Value.Trim();
                var level = layerValue.Split(',').Select(x => int.Parse(x) - 1).ToArray();
                mapLayers.Add(level);
            }

            return new TilemapModel()
            {
                Vertices = vertices,
                MapLayers = mapLayers,
                Width = width,
                Height = height,
                TileWidth = tileWidth,
                TileHeight = tileHeight
            };
        }
    }
}
