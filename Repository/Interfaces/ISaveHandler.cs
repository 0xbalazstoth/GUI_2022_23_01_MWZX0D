using Model.Game.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ISaveHandler
    {
        void Save(string saveName, IGameModel gameModel);
        void NewGame(string saveName);
        string[] LoadSaves();
        IGameModel LoadSave(string saveName);
    }
}
