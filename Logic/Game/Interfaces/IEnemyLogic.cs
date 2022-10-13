using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Interfaces
{
    public interface IEnemyLogic
    {
        void UpdateDeltaTile(float dt);
        void HandleMovement();
        void UpdateAnimationTextures(float dt, Texture[] texture, IntRect[] textureRect);
        void LoadTexture(string filename);
        void LoadTexture(Texture filename);
        void ChasePlayer();
    }
}
