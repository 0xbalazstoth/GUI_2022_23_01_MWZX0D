using Model.Game.Classes;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Interfaces
{
    public interface IBulletLogic
    {
        void UpdatePlayerBullets();
        void HandleMapCollision(RenderWindow window);
        void HandleObjectCollision(Sprite item);
        void UpdateBulletAnimationTextures();
    }
}
