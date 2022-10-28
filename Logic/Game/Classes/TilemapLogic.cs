﻿using Logic.Game.Interfaces;
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
    // Possible tiles:
    // - https://mportorodrigo.itch.io/the-lost-dungeon-tileset
    // 

    public class TilemapLogic : ITilemapLogic
    {
        public const int COLLISION_LAYER = 1;
        private IGameModel gameModel;
        
        public TilemapLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;

            gameModel.Map = new TilemapModel();
            gameModel.KillArenaMap = new TilemapModel();
        }

        public int[] CollisionMapGeneration(uint height, uint width, float scale, int seed = 209323094)
        {
            SimplexNoise.Noise.Seed = new Random().Next(1111, 999999);
            //int height = 10, width = 15;
            //float scale = 0.10f;
            float[,] noiseValues = SimplexNoise.Noise.Calc2D((int)width, (int)height, scale);

            int grassType1 = 1;
            int grassType2 = 2;
            int grassType3 = 3;
            int wall = 4;
            int grassType4 = 5;

            int[] generatedMap = new int[height * width];

            // Generate map by noise values and set tile textures by percentage
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (noiseValues[x, y] < 10f)
                    {
                        generatedMap[x + y * height] = grassType1;
                    }
                    else if (noiseValues[x, y] < 20f)
                    {
                        generatedMap[x + y * height] = grassType2;
                    }
                    else if (noiseValues[x, y] < 30f)
                    {
                        generatedMap[x + y * height] = grassType3;
                    }
                    else if (noiseValues[x, y] < 40f)
                    {
                        generatedMap[x + y * height] = grassType3;
                    }
                    else
                    {
                        generatedMap[x + y * height] = wall;
                    }
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

            int grassType1 = 1;
            int grassType2 = 2;
            int grassType3 = 3;
            int wall = 4;

            int[] generatedMap = new int[height * width];

            // Generate map by noise values and set tile textures by percentage
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (noiseValues[x, y] < 70f)
                    {
                        generatedMap[x + y * height] = grassType1;
                    }
                    else if (noiseValues[x, y] < 90f)
                    {
                        generatedMap[x + y * height] = wall;
                    }
                    else if (noiseValues[x, y] < 150f)
                    {
                        generatedMap[x + y * height] = grassType2;
                    }
                }
            }
            
            return generatedMap;
        }

        public int GetTileID(int layer, int x, int y)
        {
            var paramsAreInvalid = layer < 0 || layer >= gameModel.Map.MapLayers.Count ||
                x < 0 || x >= gameModel.Map.Width ||
                y < 0 || y >= gameModel.Map.Height;

            return paramsAreInvalid ? -1 : gameModel.Map.MapLayers[layer][y * (int)gameModel.Map.Width + x];
        }
        public Vector2f GetTileWorldPosition(int x, int y)
        {
            return new(x * gameModel.Map.TileWidth, y * gameModel.Map.TileHeight);
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
    }
}
