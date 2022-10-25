using Model.Game.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ITilemapRepository
    {
        TilemapModel LoadTMXFile(string tmxFile);
    }
}
