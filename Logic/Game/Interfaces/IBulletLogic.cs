﻿using Model.Game.Classes;
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
        void UpdateEnemiesBullets();
        void HandlePlayerBulletMapCollision(RenderWindow window);
        void HandleEnemiesBulletMapCollision(RenderWindow window);
        void HandlePlayerBulletObjectCollision(Sprite item);
        void HandleEnemiesBulletObjectCollision(Sprite item);
        void UpdatePlayerBulletAnimationTextures();
        void UpdateEnemiesBulletAnimationTextures();
    }
}
