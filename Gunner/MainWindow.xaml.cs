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
        
        private Bullet bullet;
        private List<Bullet> bullets;
        private Vector2f playerCenter;
        private Vector2f mousePosWindow;
        private Vector2f aimDir;
        private Vector2f aimDirNorm;

        private IUILogic uiLogic;
        private IUIModel uiModel;
        private UIRenderer uiRenderer;

        private Animation playerIdleAnimation;
        private Animation playerWalkDownAnimation;
        private Animation playerWalkLeftAnimation;
        private Animation playerWalkRightAnimation;
        private Animation playerWalkUpAnimation;
        private Texture[] playerTextures;
        private IntRect[] playerTextureRects;

        private Vector2f worldPos;

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
            this.playerLogic = new PlayerLogic(gameModel, tilemapLogic, WINDOW_WIDTH, WINDOW_HEIGHT);

            this.gameLogic = new GameLogic(gameModel, tilemapLogic, playerLogic, enemyLogic, chestLogic);
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
            float playerAnimationSpeed = 10f;

            playerIdleAnimation.Speed = playerAnimationSpeed;
            playerIdleAnimation.Row = 0;

            playerWalkLeftAnimation.Speed = playerAnimationSpeed;
            playerWalkLeftAnimation.Row = 1;

            playerWalkRightAnimation.Speed = playerAnimationSpeed;
            playerWalkRightAnimation.Row = 2;

            playerWalkDownAnimation.Speed = playerAnimationSpeed;
            playerWalkDownAnimation.Row = 0;

            playerWalkUpAnimation.Speed = playerAnimationSpeed;
            playerWalkUpAnimation.Row = 3;

            playerLogic.LoadTexture("player.png");

            enemyLogic = new EnemyLogic(gameModel);
            enemyLogic.LoadTexture("player.png");

            chestLogic = new ObjectEntityLogic(gameModel);
            chestLogic.LoadTexture("chest.png");
            
            gameModel.Chests[0].Position = new Vector2f(100, 100);

            bullet = new Bullet();
            bullets = new List<Bullet>();
            bullets.Add(bullet);
        }

        private void InitSystem()
        {
            window.SetFramerateLimit(60);

            playerIdleAnimation = new Animation();
            playerIdleAnimation.Load("spritesheet.png", 4, 3);

            playerWalkDownAnimation = new Animation();
            playerWalkDownAnimation.Load("spritesheet.png", 4, 3);

            playerWalkLeftAnimation = new Animation();
            playerWalkLeftAnimation.Load("spritesheet.png", 4, 3);

            playerWalkRightAnimation = new Animation();
            playerWalkRightAnimation.Load("spritesheet.png", 4, 3);

            playerWalkUpAnimation = new Animation();
            playerWalkUpAnimation.Load("spritesheet.png", 4, 3);

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

        private void GameController()
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

            playerLogic.HandleMovement(direction);
        }

        public void Update()
        {
            playerLogic.UpdateWorldPositionByMouse(window);

            // shoot
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                bulletLogic.Shoot();
                //Bullet tempBullet = new Bullet();
                //tempBullet.shape.Position = gameModel.Player.Center;
                //tempBullet.currVelocity = gameModel.Player.AimDirectionNormalized * tempBullet.maxSpeed;
                //bullets.Add(tempBullet);
            }

            bulletLogic.Update();
            //// update bullets
            //for (int i = 0; i < bullets.Count; i++)
            //{
            //    bullets[i].shape.Position += bullets[i].currVelocity;

            //    float distX = bullets[i].shape.Position.X - gameModel.Player.Center.X;
            //    float distY = bullets[i].shape.Position.Y - gameModel.Player.Center.Y;

            //    // remove bullets that go off screen
            //    if (Math.Sqrt(distX * distX + distY * distY) > 1000)
            //    {
            //        bullets.RemoveAt(i);
            //    }
            //}

            uiLogic.UpdateFPS(gameLogic.GetDeltaTime);

            playerIdleAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkUpAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkDownAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkLeftAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkRightAnimation.Update(gameLogic.GetDeltaTime, 3);

            GameController();

            gameLogic.UpdateCamera(gameModel.CameraView);
            gameLogic.MoveCamera(gameModel.Map.GetMapWidth, gameModel.Player.Position, worldPos, gameLogic.GetDeltaTime);
            gameLogic.UpdatePlayer();
        }

        public void DrawGame()
        {
            gameRenderer.Draw(window);

            //foreach (Bullet bullet in bullets)
            //{
            //    window.Draw(bullet.shape);
            //}

            foreach (var bullet in gameModel.Bullets)
            {
                window.Draw(bullet.Shape);
            }

            playerTextures = new Texture[] { playerIdleAnimation.Texture, playerWalkDownAnimation.Texture, playerWalkLeftAnimation.Texture, playerWalkUpAnimation.Texture, playerWalkRightAnimation.Texture };
            playerTextureRects = new IntRect[] { playerIdleAnimation.TextureRect, playerWalkDownAnimation.TextureRect, playerWalkLeftAnimation.TextureRect, playerWalkUpAnimation.TextureRect, playerWalkRightAnimation.TextureRect };

            playerLogic.UpdateAnimationTextures(gameLogic.GetDeltaTime, playerTextures, playerTextureRects);
        }

        public void DrawUI()
        {
            //uiRenderer.Draw(window);
        }
    }
}
