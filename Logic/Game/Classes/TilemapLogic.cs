using Logic.Game.Interfaces;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
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

            gameModel.CollectibleItems = new List<ICollectibleItem>();
            for (int i = 0; i < 10; i++)
            {
                CollectibleItemModel item = new CollectibleItemModel();
                item.Item = new Sprite();
                item.Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                item.ItemType = Model.Game.Enums.ItemType.Coin;
                item.Id = Guid.NewGuid();
                gameModel.CollectibleItems.Add(item);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                    {
                        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                        j = 0;
                    }
                }
            }
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
