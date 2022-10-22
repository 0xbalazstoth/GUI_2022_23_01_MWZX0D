﻿using Model.Game.Classes;
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
        private IGameModel gameModel;

        public UIRenderer(IUIModel uiModel, IGameModel gameModel, string fontPath, string fontFile)
        {
            this.uiModel = uiModel;
            this.gameModel = gameModel;

            uiModel.PlayerCoinSprite.Texture = new Texture(@"Assets\Textures\coin.png");
            uiModel.Font = new Font(Path.Combine(fontPath, fontFile));

            uiModel.FPSText.Font = uiModel.Font;
            uiModel.PlayerAmmoText.Font = uiModel.Font;
            uiModel.PlayerXPLevelText.Font = uiModel.Font;
            uiModel.PlayerCoinText.Font = uiModel.Font;
            uiModel.SpeedPotionTimer.Font = uiModel.Font;

            gameModel.Player.HPText.Font = uiModel.Font;

            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].HPText = new Text();
                gameModel.Enemies[i].HPText.Font = uiModel.Font;
            }
        }

        public void Draw(RenderTarget window)
        {
            window.Draw(DrawableFPSText());
            window.Draw(DrawableAmmoText());
            window.Draw(DrawableXPLevelText());
            window.Draw(DrawablePlayerCoinSprite());
            window.Draw(DrawablePlayerCoinText());
            window.Draw(DrawableSpeedPotionTimerText());

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

        private Drawable DrawableSpeedPotionTimerText()
        {
            return uiModel.SpeedPotionTimer;
        }
    }
}
