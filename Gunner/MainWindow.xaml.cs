using Logic.Game;
using Logic.Game.Classes;
using Logic.Game.Interfaces;
using Logic.Tools;
using Model.Game;
using Model.Game.Classes;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private SFMLSurface sfmlSurface;

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

        private IUILogic uiLogic;
        private IUIModel uiModel;
        private UIRenderer uiRenderer;

        private RectangleShape enemy;
        private List<RectangleShape> enemies;
        int spawnCounter = 20;

        private TimeSpan lastRenderTime;

        public MainWindow()
        {
            InitializeComponent();

            sfmlSurface = new SFMLSurface();
            SfmlSurfaceHost.Child = sfmlSurface;
            window = new RenderWindow(sfmlSurface.Handle);

            System.Windows.Media.CompositionTarget.Rendering += RunGame;

            this.gameModel = new GameModel();
            this.uiModel = new UIModel();

            this.tilemapLogic = new TilemapLogic(gameModel);
            this.bulletLogic = new BulletLogic(gameModel);
            
            this.playerLogic = new PlayerLogic(gameModel, tilemapLogic, animationLogic, WINDOW_WIDTH, WINDOW_HEIGHT);
            this.animationLogic = new AnimationLogic(gameModel);

            this.gameLogic = new GameLogic(gameModel, tilemapLogic, playerLogic, enemyLogic, chestLogic, bulletLogic);
            this.uiLogic = new UILogic(uiModel);

            this.gameLogic.SetTilemap("map.tmx", "tilemap.png");

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            this.gameRenderer = new GameRenderer(gameModel, System.IO.Path.Combine(projectDirectory, "Assets/Textures"));

            this.uiRenderer = new UIRenderer(uiModel, System.IO.Path.Combine(projectDirectory, "Assets/Fonts"), "FreeMono.ttf");

            InitSystem();
            InitGameplay();
        }

        private void InitGameplay()
        {
            playerLogic.LoadTexture("player.png");

            enemyLogic = new EnemyLogic(gameModel);
            enemyLogic.LoadTexture("player.png");

            chestLogic = new ObjectEntityLogic(gameModel);
            chestLogic.LoadTexture("chest.png");

            gameModel.Chests[0].Position = new Vector2f(100, 100);

            enemy = new RectangleShape();
            enemy.Size = new Vector2f(32, 32);
            enemy.FillColor = Color.Red;

            enemies = new List<RectangleShape>();
            enemies.Add(enemy);
        }

        private void InitSystem()
        {
            window.SetFramerateLimit(144);
            
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
            System.Windows.Media.RenderingEventArgs args = (System.Windows.Media.RenderingEventArgs)e;
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

        private void GamePlayerControl()
        {
            Dictionary<Key, Vector2f> input = new()
            {
               { Key.W, gameModel.MovementDirections[MovementDirection.Up].Direction },
               { Key.S, gameModel.MovementDirections[MovementDirection.Down].Direction },
               { Key.A, gameModel.MovementDirections[MovementDirection.Left].Direction },
               { Key.D, gameModel.MovementDirections[MovementDirection.Right].Direction },
            };

            Vector2f direction = new();
            foreach (var kvp in input)
            {
                if (IsKeyPressed(kvp.Key))
                    direction += kvp.Value;
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                bulletLogic.Shoot();
            }

            playerLogic.HandleMovement(direction);
        }

        public void Update()
        {
            gameLogic.UpdateBullets(window);

            if (spawnCounter < 20)
            {
                spawnCounter++;
            }

            if (spawnCounter >= 20 && enemies.Count < 50)
            {
                enemy.Position = new Vector2f(new Random().Next(0, 1000), new Random().Next(0, 1000));
                enemies.Add(enemy);

                spawnCounter = 0;
            }

            uiLogic.UpdateFPS(gameLogic.GetDeltaTime);

            animationLogic.Update(gameLogic.GetDeltaTime, 4);

            playerLogic.UpdateAnimationTextures();

            GamePlayerControl();

            gameLogic.UpdateCamera(gameModel.CameraView);
            gameLogic.MoveCamera(gameModel.Map.GetMapWidth, gameLogic.GetDeltaTime);
            gameLogic.UpdatePlayer(window);
        }

        public void DrawGame()
        {
            gameRenderer.Draw(window);

            foreach (var enemy in enemies)
            {
                window.Draw(enemy);
            }
        }

        public void DrawUI()
        {
            uiRenderer.Draw(window);
        }
    }
}
