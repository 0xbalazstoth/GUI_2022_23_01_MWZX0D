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

        public MenuUIRenderer(IMenuUIModel menuUIModel, string fontPath, string fontFile)
        {
            this.menuUIModel = menuUIModel;

            menuUIModel.Font = new Font(Path.Combine(fontPath, fontFile));

            for (int i = 0; i < menuUIModel.MenuTexts.Count; i++)
            {
                menuUIModel.MenuTexts[i].Font = menuUIModel.Font;
            }
        }

        public void Draw(RenderTarget window)
        {
            DrawMenu(window);
        }

        public void DrawMenu(RenderTarget window)
        {
            foreach (var menuText in menuUIModel.MenuTexts)
            {
                window.Draw(menuText);
            }
        }
    }
}
