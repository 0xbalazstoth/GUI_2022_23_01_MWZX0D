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

namespace Gunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private RenderWindow window;
        private DispatcherTimer _timer;
        private CircleShape circle;

        public MainWindow()
        {
            InitializeComponent();

            circle = new CircleShape(20);
            circle.FillColor = Color.Red;
            circle.Position = new Vector2f(1, 1);

            var surf = new DrawingSurface();
            this.FormHost.Child = surf;
            SetDoubleBuffered(surf);

            var context = new ContextSettings { DepthBits = 24 };
            this.window = new RenderWindow(surf.Handle, context);

            this._timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60) };
            this._timer.Tick += Timer_Tick;
            this._timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            window.DispatchEvents();
            window.Clear(Color.Green);
            window.Draw(this.circle);
            window.Display();
        }

        private void SetDoubleBuffered(DrawingSurface surf)
        {
            System.Reflection.PropertyInfo aProp =
                typeof(System.Windows.Forms.Control).GetProperty(
                "DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            aProp.SetValue(surf, true, null);
        }

        public class DrawingSurface : System.Windows.Forms.Control
        {
            protected override void OnPaint(PaintEventArgs e) {}
            protected override void OnPaintBackground(PaintEventArgs pevent){}
        }
    }
}
