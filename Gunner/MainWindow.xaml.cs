using Logic.Game;
using Logic.Game.Classes;
using Logic.Game.Entities;
using Logic.Game.Interfaces;
using Logic.Tools;
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
using System.IO;
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
        private PlayerLogic playerLogic;

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

        //private Player player;
        private Enemy enemy;
        private List<Chest> chests;

        private Vector2f worldPos;

        private TimeSpan lastRenderTime;

        public MainWindow()
        {
            InitializeComponent();

            sfmlSurface = new SFMLSurface();
            SfmlSurfaceHost.Child = sfmlSurface;
            window = new RenderWindow(sfmlSurface.Handle);

            System.Windows.Media.CompositionTarget.Rendering += CompositionTarget_Rendering;

            this.gameModel = new GameModel();
            this.uiModel = new UIModel();

            this.gameLogic = new GameLogic(gameModel, tilemapLogic, playerLogic);
            this.uiLogic = new UILogic(uiModel);

            this.gameLogic.SetTilemap("map.tmx", "tilemap.png");

            this.tilemapLogic = new TilemapLogic(gameModel);

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

            playerLogic = new PlayerLogic(gameModel, tilemapLogic) { Position = new(WINDOW_WIDTH / 2f, WINDOW_HEIGHT - 100) };
            playerLogic.LoadTexture("player.png");

            enemy = new Enemy() { Position = new(100, 250) };
            enemy.LoadTexture("player.png");

            chests = new List<Chest>();
            chests.Add(new Chest() { Position = new(WINDOW_WIDTH / 2f, WINDOW_HEIGHT / 2f) });
            chests.Add(new Chest() { Position = new(50, 100) });
            foreach (var chest in chests)
            {
                chest.LoadTexture("chest.png");
            }
        }

        private void InitSystem()
        {
            //window.SetVerticalSyncEnabled(true);
            window.SetFramerateLimit(144);

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
            gameModel.CameraView.Size = new Vector2f(600, 600);
            gameModel.CameraView.Center = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);
            gameModel.CameraView.Viewport = new FloatRect(0f, 0f, 1f, 1f);

            gameModel.UIView = new View();
            gameModel.UIView.Size = new Vector2f(600, 600);
            gameModel.UIView.Center = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);
            gameModel.UIView.Viewport = new FloatRect(0f, 0f, 1f, 1f);

            //window.Closed += (s, e) => { window.Close(); };
            //window.Resized += (s, e) =>
            //{
            //    gameModel.CameraView = new View();
            //    gameModel.CameraView.Size = new Vector2f(e.Width, e.Height);
            //    gameModel.CameraView.Center = new Vector2f(e.Width / 2f, e.Height / 2f);
            //    window.SetView(gameModel.CameraView);

            //    gameModel.UIView = new View();
            //    gameModel.UIView.Size = new Vector2f(e.Width, e.Height);
            //    gameModel.UIView.Center = new Vector2f(e.Width / 2f, e.Height / 2f);
            //};
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
            gameLogic.UpdateDeltaTime();

            window.DispatchEvents();
            window.SetActive(true);
            window.Size = new Vector2u((uint)sfmlSurface.Size.Width, (uint)sfmlSurface.Size.Height);
            window.SetView(new View(new FloatRect(0, 0, sfmlSurface.Size.Width, sfmlSurface.Size.Height)));

            Update();

            var pixelPos = Mouse.GetPosition(window);
            var worldPos = window.MapPixelToCoords(pixelPos);
            this.worldPos = worldPos;

            window.Clear();

            window.SetView(gameModel.CameraView);
            DrawGame();

            window.SetView(gameModel.UIView);
            DrawUI();

            window.SetView(window.DefaultView);

            window.Display();

            //window.DispatchEvents();

            //window.SetActive(true);
            //window.Size = new Vector2u((uint)sfmlSurface.Size.Width, (uint)sfmlSurface.Size.Height);
            //window.SetView(new View(new FloatRect(0, 0, sfmlSurface.Size.Width, sfmlSurface.Size.Height)));

            //window.Clear(Color.Blue);

            //window.Display();
        }

        public void Update()
        {
            uiLogic.UpdateFPS(gameLogic.GetDeltaTime);

            playerIdleAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkUpAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkDownAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkLeftAnimation.Update(gameLogic.GetDeltaTime, 3);
            playerWalkRightAnimation.Update(gameLogic.GetDeltaTime, 3);

            playerLogic.Update(gameLogic.GetDeltaTime);
            gameLogic.UpdateCamera(gameModel.CameraView);
            gameLogic.MoveCamera(gameModel.Map.GetMapWidth, playerLogic.Position, this.worldPos, gameLogic.GetDeltaTime);

            playerLogic.UpdateTilePosition(gameModel.Map);
            playerLogic.HandleMapCollision(gameModel.Map);
            playerLogic.HandleEnemyCollision(enemy);

            foreach (var chest in chests)
            {
                playerLogic.HandleItemCollision(chest);
            }
        }

        public void DrawGame()
        {
            gameRenderer.Draw(window);

            playerTextures = new Texture[] { playerIdleAnimation.Texture, playerWalkDownAnimation.Texture, playerWalkLeftAnimation.Texture, playerWalkUpAnimation.Texture, playerWalkRightAnimation.Texture };
            playerTextureRects = new IntRect[] { playerIdleAnimation.TextureRect, playerWalkDownAnimation.TextureRect, playerWalkLeftAnimation.TextureRect, playerWalkUpAnimation.TextureRect, playerWalkRightAnimation.TextureRect };

            playerLogic.RedrawTexture(gameLogic.GetDeltaTime, playerTextures, playerTextureRects);
            window.Draw(playerLogic);

            foreach (var chest in chests)
            {
                window.Draw(chest);
            }
        }

        public void DrawUI()
        {
            //uiRenderer.Draw(window);
        }
    }
}
