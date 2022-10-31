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

            for (int i = 0; i < menuUIModel.MenuTexts.Count; i++)
            {
                menuUIModel.MenuTexts[i].Font = menuUIModel.Font;
            }

            arrowKeysTexture = new Texture(@"Assets\Textures\arrow_keys.png");
            menuUIModel.ArrowKeysSprite.Texture = arrowKeysTexture;

            // Create border around menu texts
            for (int i = 0; i < menuUIModel.MenuTexts.Count; i++)
            {
                menuUIModel.MenuTexts[i].OutlineColor = Color.Black;
                menuUIModel.MenuTexts[i].OutlineThickness = 2;
            }

            menuUIModel.GameNameText.Font = menuUIModel.Font;
        }

        public void Draw(RenderTarget window)
        {
            DrawMenu(window);
        }

        public void DrawMenu(RenderTarget window)
        {
            window.Draw(menuUIModel.GameNameText);

            foreach (var menuText in menuUIModel.MenuTexts)
            {
                window.Draw(menuText);
            }

            window.Draw(menuUIModel.ArrowKeysSprite);
        }
    }
}
