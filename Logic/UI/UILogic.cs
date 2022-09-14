using Logic.Tools;
using Model.UI;
using SFML.Graphics;
using System;
using System.Collections.Generic;
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

        public float GetFps { get => fps; }
        public float GetFrameTime { get => frameTime; }

        public UILogic(IUIModel uiModel)
        {
            this.uiModel = uiModel;

            uiModel.FPSText = new Text();
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
    }
}
