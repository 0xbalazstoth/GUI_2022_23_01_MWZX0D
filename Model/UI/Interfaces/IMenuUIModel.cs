using Model.Game.Enums;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UI.Interfaces
{
    public interface IMenuUIModel
    {
        public List<Text> MainMenuTexts { get; set; }
        public List<Text> PauseMenuTexts { get; set; }
        public Font Font { get; set; }
        public MenuOptionsState SelectedMenuOptionState { get; set; }
        public Sprite ArrowKeysSprite { get; set; }
        public Text GameNameText { get; set; }
        public SoundBuffer SelectSoundBuffer { get; set; }
        public Sound SelectSound { get; set; }
    }
}
