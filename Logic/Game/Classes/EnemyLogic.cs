using Logic.Game.Interfaces;
using Model.Game.Classes;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Classes
{
    public class EnemyLogic : IEnemyLogic
    {
        private IGameModel gameModel;

        public EnemyLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;

            this.gameModel.Enemy.Position = new Vector2f(100, 250);
        }

        public void HandleMovement()
        {
            
        }

        public void LoadTexture(string filename)
        {
            gameModel.Enemy.Texture = new Texture(filename);
            gameModel.Enemy.Origin = new Vector2f(gameModel.Enemy.Texture.Size.X / 2, gameModel.Enemy.Texture.Size.Y / 2);
        }

        public void LoadTexture(Texture texture)
        {
            
        }

        public void UpdateAnimationTextures(float dt, Texture[] texture, IntRect[] textureRect)
        {
            
        }

        public void UpdateDeltaTile(float dt)
        {
            gameModel.Enemy.DeltaTime = dt;
            gameModel.Enemy.Speed = 150f;
        }
    }
}
