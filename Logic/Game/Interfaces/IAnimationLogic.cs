using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Interfaces
{
    public interface IAnimationLogic
    {
        //void LoadTexture(string filename, uint totalRows, uint totalColumns);
        void Update(float dt);
        void CollectibleItemAnimationSetup();
    }
}
