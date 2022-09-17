using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Interfaces
{
    public interface IObjectEntityLogic
    {
        void UpdateDeltaTime(float dt);
        void LoadTexture(string filename);
        void LoadTexture(Texture filename);
    }
}
