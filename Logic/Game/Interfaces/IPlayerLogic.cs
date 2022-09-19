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
        void UpdateAnimationTextures(float dt, Texture[] texture, IntRect[] textureRect);
        Vector2f GetDirectionFromInput(Vector2f direction);
        MovementDirection GetMovementByDirection(Vector2f movementDirection);
        void UpdateTilePosition(TilemapModel tilemap);
        void UpdateWorldPositionByMouse(RenderWindow window);
        void LoadTexture(string filename);
        void LoadTexture(Texture filename);
        void HandleEnemyCollision(EnemyModel enemy);
        void HandleObjectCollision(Sprite item);
        void HandleMapCollision(TilemapModel tilemap);
    }
}
