﻿using Model.Game.Classes;
using Model.UI;
using Model.UI.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
    public class GameUIRenderer
    {
        private IGameUIModel uiModel;
        private IGameModel gameModel;

        public GameUIRenderer(IGameUIModel uiModel, IGameModel gameModel, string fontPath, string fontFile)
        {
            this.uiModel = uiModel;
            this.gameModel = gameModel;

            uiModel.PlayerCoinSprite.Texture = new Texture(@"Assets\Textures\coin.png");
            uiModel.PlayerSpeedSprite.Texture = new Texture(@"Assets\Textures\speed_potion.png");
            
            uiModel.Font = new Font(Path.Combine(fontPath, fontFile));

            uiModel.FPSText.Font = uiModel.Font;
            uiModel.PlayerAmmoText.Font = uiModel.Font;
            uiModel.PlayerXPLevelText.Font = uiModel.Font;
            uiModel.PlayerCoinText.Font = uiModel.Font;
            uiModel.PlayerSpeedTimerText.Font = uiModel.Font;
            uiModel.PlayerKillCountText.Font = uiModel.Font;
            uiModel.PlayerDeathCountText.Font = uiModel.Font;
            uiModel.GameOverText.Font = uiModel.Font;
            uiModel.GameWonText.Font = uiModel.Font;

            gameModel.Player.HPText.Font = uiModel.Font;

            for (int i = 0; i < gameModel.Gates.Count; i++)
            {
                for (int j = 0; j < gameModel.Gates[i].GateTexts.Count; j++)
                {
                    gameModel.Gates[i].GateTexts[j].Font = uiModel.Font;
                }
            }

            for (int i = 0; i < gameModel.CreatorTexts.Count; i++)
            {
                gameModel.CreatorTexts[i].Font = uiModel.Font;
            }

            for (int i = 0; i < gameModel.SettingsTexts.Count; i++)
            {
                gameModel.SettingsTexts[i].Font = uiModel.Font;
            }
        }

        public void Draw(RenderTarget window)
        {
            if (gameModel.Player.IsDead == false && gameModel.Player.IsGameWon == false)
            {
                window.Draw(DrawableFPSText());
                window.Draw(DrawableAmmoText());
                window.Draw(DrawableXPLevelText());
                window.Draw(DrawablePlayerCoinSprite());
                window.Draw(DrawablePlayerCoinText());
                window.Draw(DrawableSpeedPotionTimerText());

                if (gameModel.Player.IsSpeedPotionIsInUse)
                {
                    window.Draw(DrawableSpeedPotionSprite());
                }

                window.Draw(DrawablePlayerKillCountText());
                window.Draw(DrawablePlayerDeathCountText());
            }
            
            if (gameModel.Player.IsDead == true)
            {
                window.Draw(DrawableGameOverText());
            }

            if (gameModel.Player.IsGameWon)
            {
                window.Draw(DrawableGameWonText());
            }
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

        private Drawable DrawableSpeedPotionSprite()
        {
            return uiModel.PlayerSpeedSprite;
        }

        private Drawable DrawableSpeedPotionTimerText()
        {
            return uiModel.PlayerSpeedTimerText;
        }

        private Drawable DrawablePlayerKillCountText()
        {
            return uiModel.PlayerKillCountText;
        }

        private Drawable DrawableGameOverText()
        {
            return uiModel.GameOverText;
        }

        private Drawable DrawablePlayerDeathCountText()
        {
            return uiModel.PlayerDeathCountText;
        }

        private Drawable DrawableGameWonText()
        {
            return uiModel.GameWonText;
        }
    }
}
