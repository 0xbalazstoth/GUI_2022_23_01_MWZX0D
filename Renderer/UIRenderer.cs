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

namespace Renderer
{
    public class UIRenderer
    {
        private IUIModel uiModel;

        public UIRenderer(IUIModel uiModel)
        {
            this.uiModel = uiModel;
        }

        public void Draw(RenderTarget window)
        {
            uiModel.PlayerCoinSprite.Texture = new Texture(@"Assets\Textures\coin.png");

            window.Draw(DrawableFPSText());
            window.Draw(DrawableAmmoText());
            window.Draw(DrawableXPLevelText());
            window.Draw(DrawablePlayerCoinSprite());
            window.Draw(DrawablePlayerCoinText());
        }

        private Drawable DrawableFPSText()
        {
            return uiModel.FPSText;
        }

        private Drawable DrawableAmmoText()
        {
            return uiModel.PlayerAmmoText;
        }

        private Drawable DrawableXPLevelText()
        {
            return uiModel.PlayerXPLevelText;
        }

        private Drawable DrawablePlayerCoinSprite()
        {
            return uiModel.PlayerCoinSprite;
        }

        private Drawable DrawablePlayerCoinText()
        {
            return uiModel.PlayerCoinText;
        }
    }
}
