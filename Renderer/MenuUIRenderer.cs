using Model.UI.Interfaces;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
    public class MenuUIRenderer
    {
        private IMenuUIModel menuUIModel;
        private Texture arrowKeysTexture;

        public MenuUIRenderer(IMenuUIModel menuUIModel, string fontPath, string fontFile)
        {
            this.menuUIModel = menuUIModel;

            menuUIModel.Font = new Font(Path.Combine(fontPath, fontFile));

            for (int i = 0; i < menuUIModel.MainMenuTexts.Count; i++)
            {
                menuUIModel.MainMenuTexts[i].Font = menuUIModel.Font;
                menuUIModel.MainMenuTexts[i].OutlineColor = Color.Black;
                menuUIModel.MainMenuTexts[i].OutlineThickness = 2;
            }

            for (int i = 0; i < menuUIModel.PauseMenuTexts.Count; i++)
            {
                menuUIModel.PauseMenuTexts[i].Font = menuUIModel.Font;
                menuUIModel.PauseMenuTexts[i].OutlineColor = Color.Black;
                menuUIModel.PauseMenuTexts[i].OutlineThickness = 2;
            }

            arrowKeysTexture = new Texture(@"Assets\Textures\arrow_keys.png");
            menuUIModel.ArrowKeysSprite.Texture = arrowKeysTexture;

            menuUIModel.GameNameText.Font = menuUIModel.Font;
        }

        public void DrawMainMenu(RenderTarget window)
        {
            window.Draw(menuUIModel.GameNameText);

            foreach (var menuText in menuUIModel.MainMenuTexts)
            {
                window.Draw(menuText);
            }

            window.Draw(menuUIModel.ArrowKeysSprite);
        }

        public void DrawPauseMenu(RenderTarget window)
        {
            window.Draw(menuUIModel.GameNameText);

            foreach (var menuText in menuUIModel.PauseMenuTexts)
            {
                window.Draw(menuText);
            }

            window.Draw(menuUIModel.ArrowKeysSprite);
        }
    }
}
