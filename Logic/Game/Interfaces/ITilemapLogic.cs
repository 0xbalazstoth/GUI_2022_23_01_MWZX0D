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
    }
}
