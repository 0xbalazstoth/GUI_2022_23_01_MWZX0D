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
        void UpdateDeltaTime(float dt);
        void HandleMovement();
        void UpdateAnimationTextures();
        void LoadTexture(string filename);
        void LoadTexture(Texture filename);
        void PathToPlayer();
        void HandleBulletCollision();
        void UpdateHP();
        void Shoot(int enemyIdx);
        void FlipAndRotateGun();
        void CreateEnemies();
        void SpawnEnemies();
        void ReloadGun(int enemyIdx);
    }
}
