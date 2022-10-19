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

            gameModel.Map = new TilemapModel();
        }

        public void Generation(int seed = 209323094)
        {
            SimplexNoise.Noise.Seed = new Random().Next(1111, 999999);
            int length = 10, width = 15;
            float scale = 0.10f;
            float[,] noiseValues = SimplexNoise.Noise.Calc2D(length, width, scale);

            int grassType1 = 1;
            int grassType2 = 2;
            int grassType3 = 3;
            int wall = 5;

            int[] generatedMap = new int[length * width];

            // Generate map by noise values and set tile textures by percentage
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (noiseValues[x, y] < 70f)
                    {
                        generatedMap[x + y * length] = grassType1;
                    }
                    else if (noiseValues[x, y] < 90f)
                    {
                        generatedMap[x + y * length] = grassType2;
                    }
                    else if (noiseValues[x, y] < 150f)
                    {
                        generatedMap[x + y * length] = wall;
                    }
                    else
                    {
                        generatedMap[x + y * length] = grassType3;
                    }
                }
            }

            // Print generated map
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Trace.Write(generatedMap[x + y * length] + " ");
                }
                Trace.WriteLine("");
            }

            var map = gameModel.Map;
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
    }
}
