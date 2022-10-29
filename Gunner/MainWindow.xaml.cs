using Gunner.Controller;
using Logic.Game;
using Logic.Game.Classes;
using Logic.Game.Interfaces;
using Logic.Tools;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Enums;
using Model.Tools;
using Model.UI.Classes;
using Model.UI.Interfaces;
using Renderer;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static SFML.Window.Keyboard;
using Color = SFML.Graphics.Color;
using Keyboard = SFML.Window.Keyboard;
using MessageBox = System.Windows.MessageBox;
using Mouse = SFML.Window.Mouse;
using View = SFML.Graphics.View;

namespace Gunner
{
    public static class CompositionTargetEx
    {
        private static TimeSpan _last = TimeSpan.Zero;
        private static event EventHandler<RenderingEventArgs> _FrameUpdating;
        public static event EventHandler<RenderingEventArgs> Rendering
        {
            add
            {
                if (_FrameUpdating == null)
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                _FrameUpdating += value;
            }
            remove
            {
                _FrameUpdating -= value;
                if (_FrameUpdating == null)
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }
        static void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;
            if (args.RenderingTime == _last)
                return;
            _last = args.RenderingTime; _FrameUpdating(sender, args);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private SFMLSurface sfmlSurface;
        private WindowsFormsHost host;

        private const uint WINDOW_WIDTH = 600;
        private const uint WINDOW_HEIGHT = 600;

        private RenderWindow window;

        private IGameLogic gameLogic;
        private IGameModel gameModel;
        private GameRenderer gameRenderer;

        private ITilemapLogic tilemapLogic;
        private IPlayerLogic playerLogic;
        private IEnemyLogic enemyLogic;
        private IObjectEntityLogic chestLogic;
        private IBulletLogic bulletLogic;
        private IAnimationLogic animationLogic;

        private GameController gameController;

        private IUILogic uiLogic;
        private IUIModel uiModel;
        private UIRenderer uiRenderer;

        private TimeSpan lastRenderTime;

        public MainWindow(string playerUsername)
        {
            InitializeComponent();

            host = new WindowsFormsHost();
            host.Name = "SfmlSurfaceHost";
            sfmlSurface = new SFMLSurface();
            //SfmlSurfaceHost.Child = sfmlSurface;
            window = new RenderWindow(sfmlSurface.Handle);

            host.Child = sfmlSurface;
            dockPanel.Children.Add(host);

            Trace.WriteLine(playerUsername);

            //CompositionTarget.Rendering += RunGame;
            CompositionTargetEx.Rendering += RunGame;

            this.gameModel = new GameModel();
            this.uiModel = new UIModel();

            this.tilemapLogic = new TilemapLogic(gameModel);
            this.bulletLogic = new BulletLogic(gameModel, tilemapLogic);
            this.playerLogic = new PlayerLogic(gameModel, tilemapLogic);
            this.enemyLogic = new EnemyLogic(gameModel, tilemapLogic);

            this.gameLogic = new GameLogic(gameModel, tilemapLogic, playerLogic, enemyLogic, chestLogic, bulletLogic);
            this.uiLogic = new UILogic(uiModel, gameModel);

            this.animationLogic = new AnimationLogic(gameModel);

            this.gameRenderer = new GameRenderer(gameModel, "Assets/Textures");
            this.uiRenderer = new UIRenderer(uiModel, gameModel, "Assets/Fonts", "VT323.ttf");

            InitSystem();
            InitGameplay();

            this.gameController = new GameController(gameModel, playerLogic);
        }

        private void HostGame()
        {
            host = new WindowsFormsHost();
            host.Name = "SfmlSurfaceHost";
            sfmlSurface = new SFMLSurface();
            //SfmlSurfaceHost.Child = sfmlSurface;
            window = new RenderWindow(sfmlSurface.Handle);

            host.Child = sfmlSurface;
            dockPanel.Children.Add(host);

            //CompositionTarget.Rendering += RunGame;
            CompositionTargetEx.Rendering += RunGame;

            this.gameModel = new GameModel();
            this.uiModel = new UIModel();

            this.tilemapLogic = new TilemapLogic(gameModel);
            this.bulletLogic = new BulletLogic(gameModel, tilemapLogic);
            this.playerLogic = new PlayerLogic(gameModel, tilemapLogic);
            this.enemyLogic = new EnemyLogic(gameModel, tilemapLogic);

            this.gameLogic = new GameLogic(gameModel, tilemapLogic, playerLogic, enemyLogic, chestLogic, bulletLogic);
            this.uiLogic = new UILogic(uiModel, gameModel);

            this.animationLogic = new AnimationLogic(gameModel);

            this.gameRenderer = new GameRenderer(gameModel, "Assets/Textures");
            this.uiRenderer = new UIRenderer(uiModel, gameModel, "Assets/Fonts", "VT323.ttf");

            InitSystem();
            InitGameplay();

            this.gameController = new GameController(gameModel, playerLogic);
        }

        private void InitGameplay()
        {
            chestLogic = new ObjectEntityLogic(gameModel);
            chestLogic.LoadTexture("Assets/Textures/chest.png");

            (gameModel.Objects[0] as ChestModel).Position = new Vector2f(100, 100);
        }

        private void InitSystem()
        {
            window.SetFramerateLimit(144);
            //window.SetVerticalSyncEnabled(true);

            gameModel.CameraView = new View();
            gameModel.CameraView.Size = new Vector2f(WINDOW_WIDTH, WINDOW_HEIGHT);
            gameModel.CameraView.Center = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);
            gameModel.CameraView.Viewport = new FloatRect(0f, 0f, 1f, 1f);
            
            gameModel.UIView = new View();
            gameModel.UIView.Size = new Vector2f(WINDOW_WIDTH, WINDOW_HEIGHT);
            gameModel.UIView.Center = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);
            gameModel.UIView.Viewport = new FloatRect(0f, 0f, 1f, 1f);

            window.Closed += (s, e) => { window.Close(); };
            window.Resized += (s, e) =>
            {
                gameModel.CameraView = new View();
                gameModel.CameraView.Size = new Vector2f(e.Width, e.Height);
                gameModel.CameraView.Center = new Vector2f(e.Width / 2f, e.Height / 2f);
                window.SetView(gameModel.CameraView);

                gameModel.UIView = new View();
                gameModel.UIView.Size = new Vector2f(e.Width, e.Height);
                gameModel.UIView.Center = new Vector2f(e.Width / 2f, e.Height / 2f);
            };
        }
        
        private void RunGame(object? sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;
            if (args.RenderingTime != lastRenderTime)
            {
                GameLoop();
                lastRenderTime = args.RenderingTime;
            }
        }

        private void GameLoop()
        {
            gameLogic.UpdateDeltaTime();

            window.DispatchEvents();
            window.SetActive(true);
            window.Size = new Vector2u((uint)sfmlSurface.Size.Width, (uint)sfmlSurface.Size.Height);
            window.SetView(new View(new FloatRect(0, 0, sfmlSurface.Size.Width, sfmlSurface.Size.Height)));

            Update();

            window.Clear();

            window.SetView(gameModel.CameraView);
            DrawGame();

            window.SetView(gameModel.UIView);
            DrawUI();

            window.SetView(window.DefaultView);

            window.Display();
        }

        private void Control()
        {
            gameController.HandleMovementInput();
            gameController.HandleShootInput();
            gameController.HandleReloadInput();
            gameController.HandleDebugMode();
            //window.MouseWheelScrolled += (s, e) =>
            //{
            //    int gunIdx = 0;
            //    if (e.Delta > 0)
            //    {
            //        gunIdx = gameModel.Guns.IndexOf(gameModel.Player.Gun) + 1;
            //        if (gunIdx >= gameModel.Guns.Count)
            //        {
            //            gunIdx = 0;
            //        }
            //    }
            //    else if (e.Delta < 0)
            //    {
            //        gunIdx = gameModel.Guns.IndexOf(gameModel.Player.Gun) - 1;
            //        if (gunIdx < 0)
            //        {
            //            gunIdx = gameModel.Guns.Count - 1;
            //        }
            //    }

            //    //gameModel.Player.Gun = gameModel.Guns[gunIdx];
            //};
        }

        public void Update()
        {
            bool isInWindow = true;
            if (Mouse.GetPosition(window).X < 0 || Mouse.GetPosition(window).X > window.Size.X || Mouse.GetPosition(window).Y < 0 || Mouse.GetPosition(window).Y > window.Size.Y)
            {
                isInWindow = false;
            }
            else
            {
                isInWindow = true;
            }

            if (isInWindow && gameModel.Player.IsFocusedInGame)
            {
                uiLogic.UpdateFPS(gameLogic.GetDeltaTime);
                animationLogic.Update(gameLogic.GetDeltaTime);

                gameLogic.UpdateBullets(window);
                gameLogic.UpdateCamera(gameModel.CameraView);
                gameLogic.MoveCamera(gameModel.Map.GetMapWidth, gameLogic.GetDeltaTime);
                gameLogic.UpdatePlayer(window);

                gameLogic.UpdateTilemap();
                
                Control();

                gameLogic.SpawnItems();

                uiLogic.UpdateAmmoText();
                uiLogic.UpdateXPLevelText();
                uiLogic.UpdatePlayerCoinText();
                uiLogic.UpdateSpeedPotionTimeLeftText();

                gameLogic.UpdateEnemies(window);
                enemyLogic.HandleMovement();
            }
        }

        public void DrawGame()
        {
            gameRenderer.Draw(window);
        }

        public void DrawUI()
        {
            uiRenderer.Draw(window);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            gameController.HandleInventoryInput(e);

            // Check if F11 is pressed
            if (e.Key == System.Windows.Input.Key.F11)
            {
                // Check if window is in fullscreen mode
                if (WindowState == WindowState.Maximized)
                {
                    // Set window to normal mode
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.ThreeDBorderWindow;
                }
                else
                {
                    // Set window to fullscreen mode
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                }
            }
        }

        private void SfmlSurfaceHost_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //// change gun by mouse wheel from gameModel.Guns list
            //int gunIdx = 0;
            //if (e.Delta > 0)
            //{
            //    gunIdx = gameModel.Guns.IndexOf(gameModel.Player.Gun) + 1;
            //    if (gunIdx >= gameModel.Guns.Count)
            //    {
            //        gunIdx = 0;
            //    }
            //}
            //else if (e.Delta < 0)
            //{
            //    gunIdx = gameModel.Guns.IndexOf(gameModel.Player.Gun) - 1;
            //    if (gunIdx < 0)
            //    {
            //        gunIdx = gameModel.Guns.Count - 1;
            //    }
            //}

            //gameModel.Player.Gun = gameModel.Guns[gunIdx];
        }
    }
}
