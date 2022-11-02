using Model.Game.Classes;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Interfaces
{
    public interface ITilemapLogic
    {
        int GetTileID(int layer, int x, int y);
        Vector2f GetTileWorldPosition(int x, int y);
        void UpdateItemAnimationTextures();
        int[] MapGeneration(uint height, uint width, float scale, int seed = 209323094);
        int[] CollisionMapGeneration(uint height, uint width, float scale, int seed = 209323094);
        void InitializeVertices(TilemapModel map);
        Vector2f GetTextureCoordinatesByTileID(TilemapModel map, int id);
        List<Vector2f> GetTileIdCoordinatesByMapLayer(int layer, params int[] tileIds);
    }
}
