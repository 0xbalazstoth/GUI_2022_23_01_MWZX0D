﻿using Model.UI;
using Model.UI.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
    public class UIRenderer
    {
        private IUIModel uiModel;

        public UIRenderer(IUIModel uiModel, string fontPath, string fontFile)
        {
            this.uiModel = uiModel;
            var font = new Font(Path.Combine(fontPath, fontFile));

            uiModel.FPSText.FillColor = Color.Red;
            uiModel.FPSText.Position = new Vector2f(10, 10);
            uiModel.FPSText.CharacterSize = 16;
            uiModel.FPSText.Font = font;
            uiModel.Font = font;
        }

        public void Draw(RenderTarget window)
        {
            window.Draw(DrawableFPSText());
        }

        private Drawable DrawableFPSText()
        {
            return uiModel.FPSText;
        }
    }
}
