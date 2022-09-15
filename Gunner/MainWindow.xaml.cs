using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Color = SFML.Graphics.Color;
using MessageBox = System.Windows.MessageBox;
using View = SFML.Graphics.View;

namespace Gunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private SFMLSurface sfmlSurface;
        private RenderWindow window;

        private TimeSpan lastRenderTime;
        private CircleShape circle;

        public MainWindow()
        {
            InitializeComponent();

            sfmlSurface = new SFMLSurface();
            SfmlSurfaceHost.Child = sfmlSurface;
            window = new RenderWindow(sfmlSurface.Handle);

            System.Windows.Media.CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            System.Windows.Media.RenderingEventArgs args = (System.Windows.Media.RenderingEventArgs)e;
            if (args.RenderingTime != lastRenderTime)
            {
                GameLoop();
                lastRenderTime = args.RenderingTime;
            }
        }

        private void GameLoop()
        {
            window.DispatchEvents();
            window.SetActive(true);
            window.Size = new Vector2u((uint)sfmlSurface.Size.Width, (uint)sfmlSurface.Size.Height);
            window.SetView(new View(new FloatRect(0, 0, sfmlSurface.Size.Width, sfmlSurface.Size.Height)));

            window.Clear(Color.Blue);

            if (circle == null)
            {
                circle = new CircleShape(50);
                circle.FillColor = Color.Red;
                circle.Position = new Vector2f(50, 100);
            }

            window.Draw(circle);

            window.Display();
        }
    }
}
