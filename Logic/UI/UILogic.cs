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

        public UILogic(IUIModel uiModel, IGameModel gameModel)
        {
            this.uiModel = uiModel;
            this.gameModel = gameModel;
            
            uiModel.FPSText = new Text();
            uiModel.PlayerAmmoText = new Text();
            uiModel.PlayerXPLevelText = new Text();
            uiModel.PlayerCoinText = new Text();
            uiModel.PlayerCoinSprite = new Sprite();
            uiModel.PlayerSpeedTimerText = new Text();
            uiModel.PlayerSpeedSprite = new Sprite();

            uiModel.FPSText.FillColor = Color.Red;
            uiModel.FPSText.Position = new Vector2f(10, 10);
            uiModel.FPSText.CharacterSize = 16;

            uiModel.PlayerAmmoText.FillColor = Color.Green;
            uiModel.PlayerAmmoText.Position = new Vector2f(10, 50);
            uiModel.PlayerAmmoText.CharacterSize = 18;

            uiModel.PlayerXPLevelText.FillColor = Color.Yellow;
            uiModel.PlayerXPLevelText.Position = new Vector2f(10, 70);
            uiModel.PlayerXPLevelText.CharacterSize = 18;

            uiModel.PlayerCoinSprite.Scale = new Vector2f(2f, 2f);
            uiModel.PlayerCoinSprite.Position = new Vector2f(6, 90);
            uiModel.PlayerCoinText.FillColor = Color.Yellow;
            uiModel.PlayerCoinText.Position = new Vector2f(uiModel.PlayerCoinSprite.Position.X + 32, uiModel.PlayerCoinSprite.Position.Y + 4);
            uiModel.PlayerCoinText.CharacterSize = 18;

            //uiModel.PlayerSpeedSprite.Scale = new Vector2f(2f, 2f);
            uiModel.PlayerSpeedSprite.Position = new Vector2f(6, 120);
            uiModel.PlayerSpeedTimerText.FillColor = new Color(3, 240, 252);
            uiModel.PlayerSpeedTimerText.Position = new Vector2f(uiModel.PlayerSpeedSprite.Position.X + 32, uiModel.PlayerSpeedSprite.Position.Y + 4);
            uiModel.PlayerSpeedTimerText.CharacterSize = 18;
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
            uiModel.PlayerAmmoText.DisplayedString = $"Ammo in clip: {gameModel.Player.Gun.MaxAmmo}/{gameModel.Player.Gun.CurrentAmmo}";
        }

        public void UpdateXPLevelText()
        {
            uiModel.PlayerXPLevelText.DisplayedString = $"XP Level: {gameModel.Player.CurrentXP}";
        }

        public void UpdatePlayerCoinText()
        {
            uiModel.PlayerCoinText.DisplayedString = $"{gameModel.Player.CurrentCoins}";
        }

        public void UpdateSpeedPotionTimeLeftText()
        {
            var timeLeft = 11 - (DateTime.Now - gameModel.Player.LastPotionEffect).TotalSeconds;

            if (gameModel.Player.IsSpeedPotionIsInUse)
            {
                uiModel.PlayerSpeedTimerText.DisplayedString = $"{timeLeft.ToString("0")} sec";
            }
            else
            {
                uiModel.PlayerSpeedTimerText.DisplayedString = "";
            }
        }
    }
}
