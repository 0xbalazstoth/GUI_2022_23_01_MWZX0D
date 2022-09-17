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
    public class ObjectEntityLogic : IObjectEntityLogic
    {
        private IGameModel gameModel;

        public ObjectEntityLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;
        }

        public void LoadTexture(string filename)
        {
            ChestModel chestModel = new ChestModel();
            chestModel.Size = new Vector2i(32, 32);
            chestModel.Texture = new Texture(filename);
            chestModel.Origin = new Vector2f(chestModel.Texture.Size.X / 2, chestModel.Texture.Size.Y / 2);
            chestModel.Scale = new Vector2f((float)chestModel.Size.X / chestModel.Texture.Size.X, (float)chestModel.Size.Y / chestModel.Texture.Size.Y);

            gameModel.Chests.Add(chestModel);
        }

        public void LoadTexture(Texture texture)
        {
            ChestModel chestModel = new ChestModel();
            chestModel.Size = new Vector2i(32, 32);
            chestModel.Texture = texture;
            chestModel.Origin = new Vector2f(chestModel.Texture.Size.X / 2, chestModel.Texture.Size.Y / 2);
            chestModel.Scale = new Vector2f((float)chestModel.Size.X / chestModel.Texture.Size.X, (float)chestModel.Size.Y / chestModel.Texture.Size.Y);

            gameModel.Chests.Add(chestModel);
        }

        public void UpdateDeltaTime(float dt)
        {
            
        }
    }
}
