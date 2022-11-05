using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.UI.Interfaces
{
    public interface IGameUILogic
    {
        public float GetFps { get; }
        public float GetFrameTime { get; }
        public void UpdateFPS(float dt);
        public void UpdateAmmoText();
        public void UpdateXPLevelText();
        public void UpdatePlayerCoinText();
        public void UpdateSpeedPotionTimeLeftText();
        public void UpdateKillCountText();
        public void UpdateGameOverText(RenderWindow window);
        public void UpdateGateEnterText(RenderWindow window);
    }
}
