using Logic.Game.Entities;
using Model.Game;
using Model.Game.Classes;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Interfaces
{
    public interface IPlayerLogic
    {
        void UpdateDeltaTime(float dt);
        void HandleMovement();
        void UpdateAnimationTextures(float dt, Texture[] texture, IntRect[] textureRect);
        Vector2f GetDirectionFromInput();
        MovementDirection GetMovementByDirection(Vector2f movementDirection);
        void UpdateTilePosition(TilemapModel tilemap);
        void LoadTexture(string filename);
        void LoadTexture(Texture filename);
        void HandleEnemyCollision(Enemy enemy);
        void HandleItemCollision(ItemEntity item);
        void HandleMapCollision(TilemapModel tilemap);
    }
}
