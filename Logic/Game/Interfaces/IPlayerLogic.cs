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
        void HandleMovement();
        void UpdateDeltaTime(float dt);
        void UpdateAnimationTextures(float dt, Texture[] texture, IntRect[] textureRect);
        void UpdateTilePosition(TilemapModel tilemap);
        Vector2f GetDirectionFromInput();
        MovementDirection GetMovementByDirection(Vector2f movementDirection);
    }
}
