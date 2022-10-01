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
        private RectangleShape enemy2;
        private List<RectangleShape> enemies;

        private TimeSpan lastRenderTime;

        public MainWindow()
        {
            InitializeComponent();

            sfmlSurface = new SFMLSurface();
            SfmlSurfaceHost.Child = sfmlSurface;
            window = new RenderWindow(sfmlSurface.Handle);

            CompositionTarget.Rendering += RunGame;

            this.gameModel = new GameModel();
            this.uiModel = new UIModel();

            this.tilemapLogic = new TilemapLogic(gameModel);
            this.bulletLogic = new BulletLogic(gameModel, tilemapLogic);
            
            this.playerLogic = new PlayerLogic(gameModel, tilemapLogic, animationLogic, WINDOW_WIDTH, WINDOW_HEIGHT);
            this.animationLogic = new AnimationLogic(gameModel);

            this.gameLogic = new GameLogic(gameModel, tilemapLogic, playerLogic, enemyLogic, chestLogic, bulletLogic);
            this.uiLogic = new UILogic(uiModel);

            this.gameLogic.SetTilemap("map.tmx", "tilemap.png");

            this.gameRenderer = new GameRenderer(gameModel, "Assets/Textures");

            this.uiRenderer = new UIRenderer(uiModel, "Assets/Fonts", "FreeMono.ttf");

            InitSystem();
            InitGameplay();
        }

        private void InitGameplay()
        {
            enemyLogic = new EnemyLogic(gameModel);
            enemyLogic.LoadTexture("player.png");

            chestLogic = new ObjectEntityLogic(gameModel);
            chestLogic.LoadTexture("chest.png");

            (gameModel.Objects[0] as ChestModel).Position = new Vector2f(100, 100);

            enemy = new RectangleShape();
            enemy.Size = new Vector2f(32, 32);
            enemy.FillColor = Color.Red;

            enemy2 = new RectangleShape();
            enemy2.Position = new Vector2f(50, 100);
            enemy2.Size = new Vector2f(32, 32);
            enemy2.FillColor = Color.Blue;

            enemies = new List<RectangleShape>();
            enemies.Add(enemy);
            enemies.Add(enemy2);
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
                {
                    direction += kvp.Value;
                }
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                bulletLogic.Shoot();

                //for (int i = 0; i < 20; i++)
                //{
                //    gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Center.X + (float)new Random().NextDouble() * 10f - 5f, gameModel.CameraView.Center.Y + (float)new Random().NextDouble() * 10f - 5f);
                //}
            }

            playerLogic.HandleMovement(direction);

            if (IsKeyPressed(Key.I))
            {
                //playerLogic.AddItemToInventory("a" + (char)new Random().Next(97, 102));
            }
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

            if (isInWindow)
            {
                gameLogic.UpdateBullets(window);

                EnemyChasePlayer();

                uiLogic.UpdateFPS(gameLogic.GetDeltaTime);

                animationLogic.Update(gameLogic.GetDeltaTime);

                GamePlayerControl();

                gameLogic.UpdateCamera(gameModel.CameraView);
                gameLogic.MoveCamera(gameModel.Map.GetMapWidth, gameLogic.GetDeltaTime);
                gameLogic.UpdatePlayer(window);

                // Bullet collision with enemy
                foreach (var bullet in gameModel.Player.Bullets.ToList())
                {
                    foreach (var enemy in enemies)
                    {
                        if (bullet.Shape.GetGlobalBounds().Intersects(enemy.GetGlobalBounds()))
                        {
                            gameModel.Player.Bullets.Remove(bullet);
                            enemies.Remove(enemy);
                            break;
                        }
                    }
                }
            }  
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

        // ENEMY CHASE PLAYER
        public void EnemyChasePlayer()
        {
            // https://github.com/pushbuttonreceivecode/Top-Down-Shooter-Mechanics-Part-1/blob/master/main.cpp
            // https://code.markrichards.ninja/sfml/top-down-shoot-em-up-mechanics-part-1

            foreach (var enemy in enemies)
            {
                if (gameModel.Player.Position.X < enemy.Position.X)
                {
                    enemy.Position = new Vector2f(enemy.Position.X - 1, enemy.Position.Y);
                }
                else if (gameModel.Player.Position.X > enemy.Position.X)
                {
                    enemy.Position = new Vector2f(enemy.Position.X + 1, enemy.Position.Y);
                }

                if (gameModel.Player.Position.Y < enemy.Position.Y)
                {
                    enemy.Position = new Vector2f(enemy.Position.X, enemy.Position.Y - 1);
                }
                else if (gameModel.Player.Position.Y > enemy.Position.Y)
                {
                    enemy.Position = new Vector2f(enemy.Position.X, enemy.Position.Y + 1);
                }
            }
        }
    }
}
