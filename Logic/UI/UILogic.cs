using Logic.Tools;
using Model.Game.Classes;
using Model.UI;
using Model.UI.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Tools
{
    public class UILogic : IUILogic
    {
        private IUIModel uiModel;
        private float fps;
        private float frameTime;
        private float time;
        private IGameModel gameModel;

        public float GetFps { get => fps; }
        public float GetFrameTime { get => frameTime; }

        public UILogic(IUIModel uiModel, IGameModel gameModel, string fontPath, string fontFile)
        {
            this.uiModel = uiModel;
            this.gameModel = gameModel;
            var font = new Font(Path.Combine(fontPath, fontFile));
            uiModel.FPSText = new Text();
            uiModel.AmmoText = new();

            uiModel.FPSText.FillColor = Color.Red;
            uiModel.FPSText.Position = new Vector2f(10, 10);
            uiModel.FPSText.CharacterSize = 16;
            uiModel.FPSText.Font = font;



            uiModel.AmmoText.FillColor = Color.Green;
            uiModel.AmmoText.Position = new Vector2f(10, 50);
            uiModel.AmmoText.CharacterSize = 20;
            uiModel.AmmoText.Font = font;
            uiModel.Font = font;
           

        }


        public void UpdateFPS(float dt)
        {
            frameTime = dt;
            time += dt;

            if (time >= 1f)
            {
                fps = 1f / frameTime;
                time = 0;
            }

            uiModel.FPSText.DisplayedString = "FPS: " + fps.ToString();
        }

        public void UpdateAmmoText()
        {
            uiModel.AmmoText.DisplayedString = $"Ammo in clip: {gameModel.Player.Gun.MaxAmmo}/{gameModel.Player.Gun.CurrentAmmo}";
        }
    }
}
