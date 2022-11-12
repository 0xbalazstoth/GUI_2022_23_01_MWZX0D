using Model.Game;
using Model.Game.Classes;
using Model.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static SFML.Window.Keyboard;

namespace Logic.Game.Interfaces
{
    public interface IPlayerLogic
    {
        void UpdateDeltaTime(float dt);
        void HandleMovement(Vector2f direction);
        void UpdateAnimationTextures();
        Vector2f GetDirectionFromInput(Vector2f direction);
        void UpdateTilePosition(TilemapModel tilemap);
        void UpdateWorldPositionByMouse(RenderWindow window);
        void HandleEnemyCollision();
        void HandleEnemyBulletCollision();
        void HandleObjectCollision(Sprite item);
        void HandleMapCollision(TilemapModel tilemap);
        void FlipAndRotateGun();
        void AddItemToInventory(ICollectibleItem item);
        void RemoveItemFromInventory(ICollectibleItem item);
        void HandleInventory();
        void UseItemFromInventory(ICollectibleItem item);
        void ReloadGun();
        void UpdateHP();
        void Shoot();
        void UpdateSpeedPotionTimer();
        void PushbackByRecoil(float pushbackValue);
        void ShakeCameraByRecoil();
        void HandleGateCollision();
        void BuyItemFromShop(ICollectibleItem item);
    }
}
