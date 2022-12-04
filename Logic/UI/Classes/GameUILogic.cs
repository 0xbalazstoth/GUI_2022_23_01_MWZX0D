using Logic.UI.Interfaces;
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
    public class GameUILogic : IGameUILogic
    {
        private IGameUIModel uiModel;
        private float fps;
        private float frameTime;
        private float time;
        private IGameModel gameModel;

        public float GetFps { get => fps; }
        public float GetFrameTime { get => frameTime; }

        public GameUILogic(IGameUIModel uiModel, IGameModel gameModel)
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
            uiModel.PlayerKillCountText = new Text();
            uiModel.GameOverText = new Text();
            uiModel.PlayerDeathCountText = new Text();

            uiModel.FPSText.FillColor = Color.Red;
            uiModel.FPSText.Position = new Vector2f(10, 10);
            uiModel.FPSText.CharacterSize = 26;

            uiModel.PlayerAmmoText.FillColor = Color.Green;
            uiModel.PlayerAmmoText.Position = new Vector2f(10, 50);
            uiModel.PlayerAmmoText.CharacterSize = 28;

            uiModel.PlayerXPLevelText.FillColor = Color.Yellow;
            uiModel.PlayerXPLevelText.Position = new Vector2f(10, 70);
            uiModel.PlayerXPLevelText.CharacterSize = 28;

            uiModel.PlayerCoinSprite.Scale = new Vector2f(2f, 2f);
            uiModel.PlayerCoinSprite.Position = new Vector2f(6, 100);

            uiModel.PlayerCoinText.FillColor = Color.Yellow;
            uiModel.PlayerCoinText.Position = new Vector2f(uiModel.PlayerCoinSprite.Position.X + 32, uiModel.PlayerCoinSprite.Position.Y - 4);
            uiModel.PlayerCoinText.CharacterSize = 28;

            uiModel.PlayerSpeedSprite.Position = new Vector2f(6, 120);
            uiModel.PlayerSpeedTimerText.FillColor = new Color(3, 240, 252);
            uiModel.PlayerSpeedTimerText.Position = new Vector2f(uiModel.PlayerSpeedSprite.Position.X + 32, uiModel.PlayerSpeedSprite.Position.Y + 4);
            uiModel.PlayerSpeedTimerText.CharacterSize = 28;

            uiModel.PlayerKillCountText.FillColor = Color.Red;
            uiModel.PlayerKillCountText.Position = new Vector2f(10, 150);
            uiModel.PlayerKillCountText.CharacterSize = 28;

            uiModel.GameOverText.FillColor = Color.White;
            uiModel.GameOverText.CharacterSize = 90;

            uiModel.PlayerDeathCountText.FillColor = Color.Red;
            uiModel.PlayerDeathCountText.Position = new Vector2f(10, 180);
            uiModel.PlayerDeathCountText.CharacterSize = 28;
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
                uiModel.PlayerSpeedTimerText.DisplayedString = $"{timeLeft.ToString("0")} seconds";
            }
            else
            {
                uiModel.PlayerSpeedTimerText.DisplayedString = "";
            }
        }

        public void UpdateKillCountText()
        {
            uiModel.PlayerKillCountText.DisplayedString = $"Kills: {gameModel.Player.KillCounter}";
        }

        public void UpdateGameOverText(RenderWindow window)
        {
            uiModel.GameOverText.DisplayedString = "Died, press SPACE to respawn!";
            uiModel.GameOverText.Position = new Vector2f((window.Size.X / 2) - (uiModel.GameOverText.GetGlobalBounds().Width / 2), (window.Size.Y / 2) - (uiModel.GameOverText.GetGlobalBounds().Height / 2));
        }

        public void UpdateDeathCountText()
        {
            uiModel.PlayerDeathCountText.DisplayedString = $"Deaths: {gameModel.Player.DeathCounter}";
        }
    }
}
