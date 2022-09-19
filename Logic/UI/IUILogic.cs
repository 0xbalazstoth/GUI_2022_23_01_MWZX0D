using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Tools
{
    public interface IUILogic
    {
        public float GetFps { get; }
        public float GetFrameTime { get; }
        public void UpdateFPS(float dt);
    }
}
