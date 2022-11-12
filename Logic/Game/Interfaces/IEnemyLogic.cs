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
        void UpdateAnimationTextures();
        void PathToPlayer(int enemyIdx);
        void HandleBulletCollision();
        void UpdateHP();
        void Shoot(int enemyIdx);
        void FlipAndRotateGun();
        void CreateEnemies();
        void SpawnEnemies(float dt);
        void ReloadGun(int enemyIdx);
        float DistanceBetweenPlayer(int enemyIdx);
    }
}
