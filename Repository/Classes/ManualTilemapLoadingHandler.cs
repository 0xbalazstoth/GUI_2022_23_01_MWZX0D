using Model.Game.Classes;
using Repository.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository.Classes
{
    public class ManualTilemapLoadingHandler : IManualTilemapLoadingHandler
    {
        public TilemapModel LoadTMXFile(string tmxFile, string tilesetFile)
        {
            TilemapModel tilemap = new TilemapModel();
            tilemap.TilesetTexture = new Texture(tilesetFile);

            XDocument xdoc = XDocument.Load(tmxFile);
            var map = xdoc.Descendants("map");
            var layers = map.Descendants("layer");
            uint width = uint.Parse(xdoc.Element("map").Attribute("width").Value);
            uint height = uint.Parse(xdoc.Element("map").Attribute("height").Value);
            uint tileWidth = uint.Parse(xdoc.Element("map").Attribute("tilewidth").Value);
            uint tileHeight = uint.Parse(xdoc.Element("map").Attribute("tileheight").Value);

            List<int[]> mapLayers = new();
            foreach (var layer in layers)
            {
                var layerData = layer.Element("data");
                var layerValue = layerData.Value.Trim();
                var level = layerValue.Split(',').Select(x => int.Parse(x) - 1).ToArray();
                mapLayers.Add(level);
            }

            tilemap.MapLayers = mapLayers;
            tilemap.Height = height;
            tilemap.Width = width;
            tilemap.TileWidth = tileWidth;
            tilemap.TileHeight = tileHeight;

            return tilemap;
        }
    }
}
