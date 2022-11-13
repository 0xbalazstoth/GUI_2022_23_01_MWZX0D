using Logic.Game.Interfaces;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Logic.Game.Classes
{
    public class TilemapLogic : ITilemapLogic
    {
        public const int COLLISION_LAYER = 1;
        private IGameModel gameModel;
        
        public TilemapLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;

            gameModel.CurrentMap = new TilemapModel();
            gameModel.KillArenaMap = new TilemapModel();
            gameModel.LobbyMap = new TilemapModel();
        }

        public int[] GroundMapGeneration(uint height, uint width, float scale, int seed = 209323094)
        {
            SimplexNoise.Noise.Seed = new Random().Next(1111, 999999);

            int grassType1 = 169;

            int[] generatedMap = new int[height * width];

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    generatedMap[x + y * height] = grassType1;
                }
            }

            return generatedMap;
        }

        public int[] MapGeneration(uint height, uint width, float scale, int seed = 209323094)
        {
            seed = new Random().Next(1111, 999999);
            SimplexNoise.Noise.Seed = seed;
            //int height = 10, width = 15;
            //float scale = 0.10f;
            float[,] noiseValues = SimplexNoise.Noise.Calc2D((int)width, (int)height, scale);

            int[] grassTerrainSets = new int[] { 169, 168, 170, 171};

            int grassType1 = 169;
            int grassType2 = 170;
            int grassType3 = 171;
            int wallTop = 9;
            int wallBottom = 58;

            int[] generatedMap = new int[height * width];

            // Generate map by noise values and set tile textures by percentage
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    generatedMap[x + y * height] = grassTerrainSets[new Random().Next(0, grassTerrainSets.Length)];

                    if (noiseValues[x, y] < 30f)
                    {
                        generatedMap[x + y * height] = wallTop;
                    }

                    if (x == 0 && y == 0) // top left to right
                    {
                        generatedMap[x + y * height] = 1;
                    }
                    else if ((x >= 1 && x <= 10) && y == 0) // top left
                    {
                        generatedMap[x + y * height] = 2;
                    }
                    else if (x == 0 && (y >= 0 && y <= 10)) // top to bottom
                    {
                        generatedMap[x + y * height] = 98;
                    }
                    else if (x == 0 && y == 11) // bottom left
                    {
                        generatedMap[x + y * height] = 147;
                    }
                    else if ((x >= 1 && x <= 10) && y == 11) // bottom left to right
                    {
                        generatedMap[x + y * height] = 198;
                    }
                    else if (x == 11 && y == 0) // top right
                    {
                        generatedMap[x + y * height] = 3;
                    }
                    else if (x == 11 && y == 11) // bottom right
                    {
                        generatedMap[x + y * height] = 151;
                    }
                    else if (x == 11 && (y >= 0 && y <= 10) && !(y >= 4 && y <= 8)) // top to bottom
                    {
                        generatedMap[x + y * height] = 102;
                    }

                    if ((x >= 1 && x <= 10) && (y >= 1 && y <= 10))
                    {
                        generatedMap[x + y * height] = 100;
                    }
                }
            }

            return generatedMap;
        }

        public int GetTileID(int layer, int x, int y)
        {
            var paramsAreInvalid = layer < 0 || layer >= gameModel.CurrentMap.MapLayers.Count ||
                x < 0 || x >= gameModel.CurrentMap.Width ||
                y < 0 || y >= gameModel.CurrentMap.Height;

            return paramsAreInvalid ? -1 : gameModel.CurrentMap.MapLayers[layer][y * (int)gameModel.CurrentMap.Width + x];
        }
        public Vector2f GetTileWorldPosition(int x, int y)
        {
            return new(x * gameModel.CurrentMap.TileWidth, y * gameModel.CurrentMap.TileHeight);
        }

        public override string ToString()
        {
            return "[Type]: Tilemap; [Object]: Tile";
        }

        public void UpdateItemAnimationTextures()
        {
            for (int i = 0; i < gameModel.CollectibleItems.Count; i++)
            {
                gameModel.CollectibleItems[i].Item.Texture = (gameModel.CollectibleItems[i] as CollectibleItemModel).Animations[gameModel.CollectibleItems[i].ItemType].Texture;
                gameModel.CollectibleItems[i].Item.TextureRect = (gameModel.CollectibleItems[i] as CollectibleItemModel).Animations[gameModel.CollectibleItems[i].ItemType].TextureRect;
            }
        }

        public void InitializeVertices(TilemapModel map)
        {
            map.Vertices = new List<Vertex[]>();
            int corners = 4;

            for (int i = 0; i < map.MapLayers.Count; i++)
            {
                var currentVertices = new Vertex[map.Width * map.Height * corners * map.MapLayers.Count];
                for (int y = 0; y < map.Height * corners; y += corners)
                {
                    for (int x = 0; x < map.Width * corners; x += corners)
                    {
                        var tileID = map.MapLayers[i][y / corners * map.Width + x / corners];
                        var texCoords = GetTextureCoordinatesByTileID(map, tileID);
                        var tx = x / 4 * map.TileWidth;
                        var ty = y / 4 * map.TileHeight;
                        var index = y * (int)map.Width + x;

                        currentVertices[index + 0] = new(new(tx, ty), Color.White, texCoords);
                        currentVertices[index + 1] = new(new(tx + map.TileWidth, ty), Color.White, new(texCoords.X + map.TileWidth, texCoords.Y));
                        currentVertices[index + 2] = new(new(tx + map.TileWidth, ty + map.TileHeight), Color.White, new(texCoords.X + map.TileHeight, texCoords.Y + map.TileHeight));
                        currentVertices[index + 3] = new(new(tx, ty + map.TileHeight), Color.White, new(texCoords.X, texCoords.Y + map.TileHeight));
                    }
                }
                
                map.Vertices.Add(currentVertices);
            }
        }

        public Vector2f GetTextureCoordinatesByTileID(TilemapModel map, int id)
        {
            var tilesetWidth = map.TilesetTexture.Size.X / map.TileWidth;
            var tilesetHeight = map.TilesetTexture.Size.Y / map.TileHeight;
            var texCoordsX = id % tilesetWidth;
            var texCoordsY = id / tilesetWidth;
            return new Vector2f(texCoordsX * map.TileWidth, texCoordsY * map.TileHeight);
        }

        public List<Vector2f> GetTileIdCoordinatesByMapLayer(TilemapModel map, int layer, List<int> tileIds)
        {
            var points = new List<Vector2f>();

            // Get all tile coordinates by tile id from tileIds
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var tileId = map.MapLayers[layer][y * (int)map.Width + x];
                    if (tileIds.Contains(tileId))
                    {
                        points.Add(new Vector2f(x, y));
                    }
                }
            }
            //for (int i = 0; i < map.MapLayers[layer].Length; i++)
            //{
            //    for (int t = 0; t < tileIds.Count; t++)
            //    {
            //        var tileId = map.MapLayers[layer][i];
            //        var safeTileId = tileIds[t];
            //        if (tileId == safeTileId)
            //        {
            //            var textureCoordinate = GetTextureCoordinatesByTileID(map, tileIds[t]);
            //            points.Add(textureCoordinate);
            //        }
            //    }
            //}

            return points;
        }

        public List<int> GetSafeTileIDs()
        {
            var safeIDs = gameModel.CurrentMap.MapLayers[COLLISION_LAYER].Except(gameModel.CurrentMap.CollidableIDs).ToList();
            return safeIDs;
        }
    }
}
