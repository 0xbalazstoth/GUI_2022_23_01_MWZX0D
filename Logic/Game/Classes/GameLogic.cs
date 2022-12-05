using Logic.Game.Interfaces;
using Model;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Enums;
using Model.Game.Interfaces;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Repository.Interfaces;
using Repository.Classes;

namespace Logic.Game.Classes
{
    public class GameLogic : IGameLogic 
    {
        // Minden logic használhat más logicot interfacen keresztül!

        private IGameModel gameModel;
        private ITilemapLogic tilemapLogic;
        private IPlayerLogic playerLogic;
        private IEnemyLogic enemyLogic;
        private IBulletLogic bulletLogic;

        private Clock deltaTimeClock;
        private float deltaTime;
        private Music music;

        public Clock GetDeltaTimeClock { get => deltaTimeClock; }
        public float GetDeltaTime { get => deltaTime; }

        public GameLogic(IGameModel gameModel, ITilemapLogic tilemapLogic, IPlayerLogic playerLogic, IEnemyLogic enemyLogic, IBulletLogic bulletLogic)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;
            this.playerLogic = playerLogic;
            this.enemyLogic = enemyLogic;
            this.bulletLogic = bulletLogic;

            deltaTimeClock = new Clock();

            gameModel.CameraView = new View();
            gameModel.UIView = new View();

            gameModel.Objects = new List<IObjectEntity>();
            
            gameModel.MovementDirections = new Dictionary<MovementDirection, Movement>();
            gameModel.MovementDirections.Add(MovementDirection.Up, new Movement() { MovementDirection = MovementDirection.Up, Direction = new Vector2f(0, -1f) });
            gameModel.MovementDirections.Add(MovementDirection.Down, new Movement() { MovementDirection = MovementDirection.Down, Direction = new Vector2f(0, 1f) });
            gameModel.MovementDirections.Add(MovementDirection.Left, new Movement() { MovementDirection = MovementDirection.Left, Direction = new Vector2f(-1f, 0) });
            gameModel.MovementDirections.Add(MovementDirection.Right, new Movement() { MovementDirection = MovementDirection.Right, Direction = new Vector2f(1f, 0) });
            gameModel.MovementDirections.Add(MovementDirection.UpLeft, new Movement() { MovementDirection = MovementDirection.UpLeft, Direction = new Vector2f(-1f, -1f) });
            gameModel.MovementDirections.Add(MovementDirection.UpRight, new Movement() { MovementDirection = MovementDirection.UpRight, Direction = new Vector2f(1f, -1f) });
            gameModel.MovementDirections.Add(MovementDirection.DownLeft, new Movement() { MovementDirection = MovementDirection.DownLeft, Direction = new Vector2f(-1f, 1f) });
            gameModel.MovementDirections.Add(MovementDirection.DownRight, new Movement() { MovementDirection = MovementDirection.DownRight, Direction = new Vector2f(1f, 1f) });
            
            SetTilemap("Assets/Textures/map.tmx", "Assets/Textures/tilemap.png");
            
            CreateItems();
            enemyLogic.CreateEnemies(EnemyType.Eye, 5, 5, 20, 300f, 100);
            enemyLogic.CreateEnemies(EnemyType.Boss, 5, 10, 1, 900f, 1500);

            var spawnPoints = GetSafeSpawnPoints();
            Random rnd = new Random();
            int spawnPointIndex = rnd.Next(0, spawnPoints.Count);
            //gameModel.Player.Position = spawnPoints[spawnPointIndex];
            
            gameModel.Player.Position = new Vector2f(300, 500);
            gameModel.Player.PlayerState = GateState.InLobby;

            CreateMaps();

            music = new Music("Assets/Sounds/music1.ogg");
            music.Volume = 5;
            music.Play();

            gameModel.TeleportSoundBuffer = new SoundBuffer("Assets/Sounds/teleport.ogg");
            gameModel.TeleportSound = new Sound(gameModel.TeleportSoundBuffer);

            gameModel.GameOverSoundBuffer = new SoundBuffer("Assets/Sounds/gameover.ogg");
            gameModel.GameOverSound = new Sound(gameModel.GameOverSoundBuffer);

            gameModel.SpeedPotionSoundBuffer = new SoundBuffer("Assets/Sounds/speed.ogg");
            gameModel.SpeedPotionSound = new Sound(gameModel.SpeedPotionSoundBuffer);

            gameModel.HealthPotionSoundBuffer = new SoundBuffer("Assets/Sounds/health.ogg");
            gameModel.HealthPotionSound = new Sound(gameModel.HealthPotionSoundBuffer);

            gameModel.GameWonSoundBuffer = new SoundBuffer("Assets/Sounds/won.ogg");
            gameModel.GameWonSound = new Sound(gameModel.GameWonSoundBuffer);
        }

        public void SetTilemap(string tmxFile, string tilesetFile)
        {
            gameModel.CurrentMap.CollidableIDs = new List<int>();
            gameModel.CurrentMap.TilesetTexture = new Texture(tilesetFile);
            foreach (var id in gameModel.MapCollidibleIDs)
            {
                gameModel.CurrentMap.CollidableIDs.Add(id);
            }

            ManualTilemapLoadingHandler tilemapLoader = new ManualTilemapLoadingHandler();

            #region Lobby
            var lobbyMap = tilemapLoader.LoadTMXFile(tmxFile, tilesetFile);
            tilemapLogic.InitializeVertices(lobbyMap);

            gameModel.LobbyMap.CollidableIDs = new List<int>();
            gameModel.LobbyMap.Vertices = lobbyMap.Vertices;
            gameModel.LobbyMap.TilesetTexture = lobbyMap.TilesetTexture;
            gameModel.LobbyMap.MapLayers = lobbyMap.MapLayers;
            gameModel.LobbyMap.Width = lobbyMap.Width;
            gameModel.LobbyMap.Height = lobbyMap.Height;
            gameModel.LobbyMap.TileWidth = lobbyMap.TileWidth;
            gameModel.LobbyMap.TileHeight = lobbyMap.TileHeight;
            gameModel.LobbyMap.Size = new Vector2u(lobbyMap.Width, lobbyMap.Height);
            gameModel.LobbyMap.TileSize = new Vector2u(lobbyMap.TileWidth, lobbyMap.TileHeight);
            #endregion

            #region Kill arena
            uint killArenaWidth = 80;
            uint killArenaHeight = 80;
            float killArenaScale = 1f;
            uint killArenaTileWidth = 32;
            uint killArenaTileHeight = 32;
            int[] mapGeneration = tilemapLogic.MapGeneration(killArenaHeight, killArenaWidth, killArenaScale);
            int[] groundMapGeneration = tilemapLogic.GroundMapGeneration(killArenaHeight, killArenaWidth, killArenaScale);
            gameModel.KillArenaMap.CollidableIDs = new List<int>();
            gameModel.KillArenaMap.MapLayers = new List<int[]>();
            gameModel.KillArenaMap.MapLayers.Add(groundMapGeneration);
            gameModel.KillArenaMap.MapLayers.Add(mapGeneration);
            gameModel.KillArenaMap.TilesetTexture = new Texture(tilesetFile);
            gameModel.KillArenaMap.Width = killArenaWidth;
            gameModel.KillArenaMap.Height = killArenaHeight;
            gameModel.KillArenaMap.TileWidth = killArenaTileWidth;
            gameModel.KillArenaMap.TileHeight = killArenaTileHeight;
            gameModel.KillArenaMap.Size = new Vector2u(killArenaWidth, killArenaHeight);
            gameModel.KillArenaMap.TileSize = new Vector2u(killArenaTileWidth, killArenaHeight);
            tilemapLogic.InitializeVertices(gameModel.KillArenaMap);
            #endregion

            #region Boss arena
            var bossMap = tilemapLoader.LoadTMXFile("Assets/Textures/bossMap.tmx", tilesetFile);
            tilemapLogic.InitializeVertices(bossMap);

            gameModel.BossMap.CollidableIDs = new List<int>();
            gameModel.BossMap.Vertices = bossMap.Vertices;
            gameModel.BossMap.TilesetTexture = bossMap.TilesetTexture;
            gameModel.BossMap.MapLayers = bossMap.MapLayers;
            gameModel.BossMap.Width = bossMap.Width;
            gameModel.BossMap.Height = bossMap.Height;
            gameModel.BossMap.TileWidth = bossMap.TileWidth;
            gameModel.BossMap.TileHeight = bossMap.TileHeight;
            gameModel.BossMap.Size = new Vector2u(bossMap.Width, bossMap.Height);
            gameModel.BossMap.TileSize = new Vector2u(bossMap.TileWidth, bossMap.TileHeight);
            #endregion

            #region Set lobby as default map
            gameModel.CurrentMap.Vertices = gameModel.LobbyMap.Vertices;
            gameModel.CurrentMap.MapLayers = gameModel.LobbyMap.MapLayers;
            gameModel.CurrentMap.Width = gameModel.LobbyMap.Width;
            gameModel.CurrentMap.Height = gameModel.LobbyMap.Height;
            gameModel.CurrentMap.TileWidth = gameModel.LobbyMap.TileWidth;
            gameModel.CurrentMap.TileHeight = gameModel.LobbyMap.TileHeight;
            gameModel.CurrentMap.Size = new Vector2u(gameModel.LobbyMap.Width, gameModel.LobbyMap.Height);
            gameModel.CurrentMap.TileSize = new Vector2u(gameModel.LobbyMap.TileWidth, gameModel.LobbyMap.TileHeight);
            #endregion
        }

        public void UpdatePlayer(RenderWindow window)
        {
            gameModel.Player.Hitbox.Size = new Vector2f(gameModel.Player.GetGlobalBounds().Width - 15f, gameModel.Player.GetGlobalBounds().Height - 10f);
            gameModel.Player.Hitbox.Position = new Vector2f(gameModel.Player.Position.X, gameModel.Player.Position.Y);
            gameModel.Player.Hitbox.Origin = new Vector2f(gameModel.Player.Origin.X, gameModel.Player.Origin.Y);

            playerLogic.UpdateAnimationTextures();
            playerLogic.UpdateWorldPositionByMouse(window);
            playerLogic.UpdateDeltaTime(deltaTime);
            playerLogic.UpdateTilePosition(gameModel.CurrentMap);
            playerLogic.HandleMapCollision(gameModel.CurrentMap);
            playerLogic.HandleEnemyCollision();

            foreach (ObjectEntityModel chest in gameModel.Objects)
            {
                playerLogic.HandleObjectCollision(chest);
            }

            gameModel.Player.Gun.Scale = new Vector2f(2.5f, 2.5f);
            gameModel.Player.Gun.Origin = new Vector2f(gameModel.Player.Gun.Texture.Size.X / 2, gameModel.Player.Gun.Texture.Size.Y / 2);

            playerLogic.FlipAndRotateGun();
            if (gameModel.Player.PlayerState == GateState.InKillArena || gameModel.Player.PlayerState == GateState.InBossArena)
            {
                playerLogic.HandleInventory();
            }
            playerLogic.UpdateHP();
            playerLogic.UpdateSpeedPotionTimer();
            playerLogic.HandleEnemyBulletCollision();
            playerLogic.HandleGateCollision();

            if (gameModel.Player.PlayerState == GateState.InBossArena)
            {
                // Check if enemy type boss is dead
                var bossEnemy = gameModel.Enemies.Where(x => x.EnemyType == EnemyType.Boss).FirstOrDefault();

                if (bossEnemy == null)
                {
                    gameModel.Player.IsGameWon = true;
                }
                else
                {
                    gameModel.Player.IsGameWon = false;
                }
            }
        }
        
        public void UpdateBullets(RenderWindow window)
        {
            bulletLogic.HandlePlayerBulletMapCollision(window);
            bulletLogic.HandleEnemiesBulletMapCollision(window);

            foreach (ObjectEntityModel obj in gameModel.Objects)
            {
                bulletLogic.HandlePlayerBulletObjectCollision(obj);
                bulletLogic.HandleEnemiesBulletObjectCollision(obj);
            }

            bulletLogic.UpdatePlayerBullets();
            bulletLogic.UpdateEnemiesBullets();
            
            bulletLogic.UpdateEnemiesBulletAnimationTextures();
            bulletLogic.UpdatePlayerBulletAnimationTextures();
        }

        public void UpdateEnemies(RenderWindow window)
        {
            if (gameModel.Player.PlayerState == GateState.InKillArena)
            {
                if (gameModel.Enemies.Count <= 20)
                {
                    enemyLogic.CreateEnemies(EnemyType.Eye, 5, 5, 5, 300f, 100);
                }
            }

            if (gameModel.Player.PlayerState == GateState.InKillArena || gameModel.Player.PlayerState == GateState.InBossArena)
            {
                enemyLogic.UpdateAnimationTextures();
                enemyLogic.HandleBulletCollision();

                for (int i = 0; i < gameModel.Enemies.Count; i++)
                {
                    gameModel.Enemies[i].Hitbox.Size = new Vector2f(gameModel.Enemies[i].GetGlobalBounds().Width - 5f, gameModel.Enemies[i].GetGlobalBounds().Height - 5f);
                    gameModel.Enemies[i].Hitbox.Position = new Vector2f(gameModel.Enemies[i].Position.X, gameModel.Enemies[i].Position.Y);
                    gameModel.Enemies[i].Hitbox.Origin = new Vector2f(gameModel.Enemies[i].Origin.X, gameModel.Enemies[i].Origin.Y);

                    float distance = enemyLogic.DistanceBetweenPlayer(i);

                    if (distance < gameModel.Enemies[i].SightDistance)
                    {
                        enemyLogic.Shoot(i);
                        enemyLogic.PathToPlayer(i);
                    }

                    gameModel.Enemies[i].Gun.Scale = new Vector2f(2.5f, 2.5f);
                }

                enemyLogic.UpdateHP();
                enemyLogic.FlipAndRotateGun();
            }
        }

        public void UpdateTilemap()
        {
            if (gameModel.Player.PlayerState == Model.Game.Enums.GateState.InKillArena || gameModel.Player.PlayerState == Model.Game.Enums.GateState.InBossArena)
            {
                tilemapLogic.UpdateItemAnimationTextures();
            }
            
            for (int i = 0; i < gameModel.Gates.Count; i++)
            {
                if (gameModel.Gates[i].IsGateReady)
                {
                    gameModel.Gates[i].Hitbox.Size = new Vector2f(gameModel.Gates[i].GateSprite.GetGlobalBounds().Width, gameModel.Gates[i].GateSprite.GetGlobalBounds().Height - 95f);
                    gameModel.Gates[i].Hitbox.Position = new Vector2f(gameModel.Gates[i].GateSprite.Position.X, gameModel.Gates[i].GateSprite.Position.Y + 30f);
                    gameModel.Gates[i].Hitbox.Origin = new Vector2f(gameModel.Gates[i].GateSprite.Origin.X, gameModel.Gates[i].GateSprite.Origin.Y);

                    gameModel.Gates[i].InteractArea.Size = new Vector2f(gameModel.Gates[i].GateSprite.GetGlobalBounds().Width, 50f);
                    gameModel.Gates[i].InteractArea.Position = new Vector2f(gameModel.Gates[i].GateSprite.Position.X, gameModel.Gates[i].GateSprite.GetGlobalBounds().Height - 32f);

                    gameModel.Gates[i].GateSprite.Texture = gameModel.Gates[i].Animations[0].Texture;
                    gameModel.Gates[i].GateSprite.TextureRect = gameModel.Gates[i].Animations[0].TextureRect;

                    for (int j = 0; j < gameModel.Gates[i].GateTexts.Count; j++)
                    {
                        // Center texts by gate sprite
                        if (j % 2 != 0)
                        {
                            gameModel.Gates[i].GateTexts[j].Position = new Vector2f(gameModel.Gates[i].GateSprite.Position.X - (gameModel.Gates[i].Hitbox.Size.X / 2f) + 5, gameModel.Gates[i].InteractArea.Position.Y + 10f);
                        }
                        else
                        {
                            gameModel.Gates[i].GateTexts[j].Position = new Vector2f(gameModel.Gates[i].GateSprite.Position.X - (gameModel.Gates[i].Hitbox.Size.X / 2f) + 5, gameModel.Gates[i].InteractArea.Position.Y - 10f);
                        }
                    }
                }
            }
        }

        public void UpdateDeltaTime()
        {
            deltaTime = deltaTimeClock.ElapsedTime.AsSeconds();
            deltaTimeClock.Restart();

            music.Loop = true;
        }

        public void UpdateCamera(View cameraView)
        {
            gameModel.CameraView = cameraView;
        }

        public void MoveCamera(uint mapWidth, float dt)
        {
            var direction = Vector2.Normalize(new(gameModel.WorldPositionInCamera.X - gameModel.Player.Position.X, gameModel.WorldPositionInCamera.Y - gameModel.Player.Position.Y));
            var position = new Vector2(gameModel.Player.Position.X, gameModel.Player.Position.Y);
            var distance = Vector2.Distance(new(gameModel.Player.Position.X, gameModel.Player.Position.Y), new(gameModel.WorldPositionInCamera.X, gameModel.WorldPositionInCamera.Y));

            position += direction * Math.Min(distance / 3f, 100f);

            var moveDirection = Vector2.Normalize(new(position.X - gameModel.CameraView.Center.X, position.Y - gameModel.CameraView.Center.Y));
            var speed = 500f * dt;
            var result = new Vector2(gameModel.CameraView.Center.X, gameModel.CameraView.Center.Y);
            result += moveDirection * speed;

            if (Vector2.Distance(result, position) < speed * 2f)
            {
                result = position;
            }

            if (!float.IsNaN(result.X) || !float.IsNaN(result.Y))
            {
                gameModel.CameraView.Center = new Vector2f(result.X, result.Y);
            }

            // Stop camera movement on the edges of the map
            CameraEdges();
        }

        public void CreateItems()
        {
            gameModel.CollectibleItems = new List<ICollectibleItem>();

            for (int i = 0; i < new Random().Next(30, 70); i++)
            {
                CollectibleItemModel coinItem = new CollectibleItemModel();
                coinItem.Item = new Sprite();
                // Set random position for item
                // Spawn items at random position with random distance each other

                coinItem.Item.Position = new Vector2f(new Random().Next(0, (int)gameModel.KillArenaMap.GetMapWidth), new Random().Next(0, (int)gameModel.KillArenaMap.GetMapHeight));
                //coinItem.Item.Position = new Vector2f(new Random().Next(400, (int)gameModel.KillArenaMap.GetMapWidth) % gameModel.KillArenaMap.GetMapWidth, new Random().Next(400, (int)gameModel.CurrentMap.GetMapHeight) % gameModel.CurrentMap.GetMapHeight);
                coinItem.ItemType = Model.Game.Enums.ItemType.Coin;
                coinItem.CoinSoundBuffer = new SoundBuffer("Assets/Sounds/coin.ogg");
                coinItem.CoinSound = new Sound(coinItem.CoinSoundBuffer);
                coinItem.Id = (int)coinItem.ItemType;
                
                gameModel.CollectibleItems.Add(coinItem);
            }

            for (int i = 0; i < new Random().Next(30, 70); i++)
            {
                CollectibleItemModel healtPotionItem = new CollectibleItemModel();
                healtPotionItem.Item = new Sprite();
                // Set random position for item
                healtPotionItem.Item.Position = new Vector2f(new Random().Next(0, (int)gameModel.KillArenaMap.GetMapWidth), new Random().Next(0, (int)gameModel.KillArenaMap.GetMapHeight));
                //healtPotionItem.Item.Position = new Vector2f(new Random().Next(400, (int)gameModel.KillArenaMap.GetMapWidth) % gameModel.KillArenaMap.GetMapWidth, new Random().Next(400, (int)gameModel.CurrentMap.GetMapHeight) % gameModel.KillArenaMap.GetMapHeight);
                healtPotionItem.ItemType = Model.Game.Enums.ItemType.Health_Potion;
                healtPotionItem.Id = (int)healtPotionItem.ItemType;
                healtPotionItem.IconFileName = "health_potion.png";
                
                gameModel.CollectibleItems.Add(healtPotionItem);
            }

            for (int i = 0; i < new Random().Next(30, 70); i++)
            {
                CollectibleItemModel speedPotion = new CollectibleItemModel();
                speedPotion.Item = new Sprite();
                // Set random position for item
                speedPotion.Item.Position = new Vector2f(new Random().Next(0, (int)gameModel.KillArenaMap.GetMapWidth), new Random().Next(0, (int)gameModel.KillArenaMap.GetMapHeight));
                //speedPotion.Item.Position = new Vector2f(new Random().Next(400, (int)gameModel.KillArenaMap.GetMapWidth) % gameModel.KillArenaMap.GetMapWidth, new Random().Next(400, (int)gameModel.CurrentMap.GetMapHeight) % gameModel.KillArenaMap.GetMapHeight);
                speedPotion.ItemType = Model.Game.Enums.ItemType.Speed_Potion;
                speedPotion.Id = (int)speedPotion.ItemType;
                speedPotion.IconFileName = "speed_potion.png";

                gameModel.CollectibleItems.Add(speedPotion);
            }
        }

        public void SpawnItems()
        {
            foreach (var item in gameModel.CollectibleItems)
            {
                for (int y = -3; y < 3; y++)
                {
                    for (int x = -3; x < 3; x++)
                    {
                        var xTilePosition = item.Item.Position.X;
                        var yTilePosition = item.Item.Position.Y;
                        var tilePosition = new Vector2i((int)((int)xTilePosition / gameModel.CurrentMap.TileSize.X), (int)((int)yTilePosition / gameModel.CurrentMap.TileSize.Y)) + new Vector2i(x, y);
                        var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                        if (gameModel.CurrentMap.CollidableIDs.Contains(currentTileID) == false)
                        {
                            continue;
                        }

                        var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                        var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.CurrentMap.TileSize.X, gameModel.CurrentMap.TileSize.Y);
                        var rect = item.Item.GetGlobalBounds();

                        if (tileRect.Intersects(rect))
                        {
                            gameModel.CollectibleItems.Remove(item);

                            var optimalPosition = new Vector2f();
                            var optimalDistance = float.MaxValue;

                            for (int xP = 0; xP < gameModel.CurrentMap.Size.X; xP++)
                            {
                                for (int yP = 0; yP < gameModel.CurrentMap.Size.Y; yP++)
                                {
                                    var tileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, xP, yP);
                                    if (gameModel.CurrentMap.CollidableIDs.Contains(tileID) == false)
                                    {
                                        var tileWorldPosition = tilemapLogic.GetTileWorldPosition(xP, yP);
                                        var distance = Vector2.Distance(new Vector2(rect.Left, rect.Top), new Vector2(tileWorldPosition.X, tileWorldPosition.Y));
                                        if (distance < optimalDistance)
                                        {
                                            optimalDistance = distance;
                                            optimalPosition = tileWorldPosition;
                                        }
                                    }
                                }
                            }

                            item.Item.Position = optimalPosition;
                            gameModel.CollectibleItems.Add(item);

                            return;
                        }
                    }
                }

                // Spawn items inside the map
                if (item.Item.Position.X < 0 || item.Item.Position.X > gameModel.CurrentMap.Size.X * gameModel.CurrentMap.TileSize.X || item.Item.Position.Y < 0 || item.Item.Position.Y > gameModel.CurrentMap.Size.Y * gameModel.CurrentMap.TileSize.Y)
                {
                    gameModel.CollectibleItems.Remove(item);
                    return;
                }
            }
        }

        public void Music()
        {
            //Music music = new Music("Assets/Sounds/music1.ogg");
            //music.Loop = true;
            //music.Volume = 50;
            //music.Play();

            //if (gameModel.Musics == null)
            //{
            //    //gameModel.Music = new Music("Resources/Music/BackgroundMusic.ogg");
            //    //gameModel.Music.Loop = true;
            //    //gameModel.Music.Volume = 50;
            //    //gameModel.Music.Play();

            //    Music music = new Music("Resources/Music/music1.ogg");
            //    music.Loop = true;
            //    music.Volume = 50;
            //    music.Play();

            //}
            //foreach (var music in gameModel.Musics)
            //{
            //    //if (k > 1)
            //    //{
            //    //    music.Stop();
            //    //    gameModel.Musics[k - 1].Play();
            //    //}
            //    //else
            //    //{
            //    //    music.Play();
            //    //}

            //    //if (music.Status == SFML.Audio.SoundStatus.Stopped)
            //    //{
            //    //    music.Volume = 30;
            //    //    music.Play();
            //    //}
            //}

            //if (gameModel.Musics[1].Status == SoundStatus.Stopped)
            //{ 
            //    gameModel.Musics[1].Volume = 30;
            //    gameModel.Musics[1].Play();
            //}
        }

        public void CameraEdges()
        {
            if (gameModel.CameraView.Center.X < gameModel.CameraView.Size.X / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Size.X / 2f, gameModel.CameraView.Center.Y);
            }
            if (gameModel.CameraView.Center.X > gameModel.CurrentMap.GetMapWidth - gameModel.CameraView.Size.X / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CurrentMap.GetMapWidth - gameModel.CameraView.Size.X / 2f, gameModel.CameraView.Center.Y);
            }

            if (gameModel.CameraView.Center.Y < gameModel.CameraView.Size.Y / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Center.X, gameModel.CameraView.Size.Y / 2f);
            }
            if (gameModel.CameraView.Center.Y > gameModel.CurrentMap.GetMapHeight - gameModel.CameraView.Size.Y / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Center.X, gameModel.CurrentMap.GetMapHeight - gameModel.CameraView.Size.Y / 2f);
            }
        }

        public List<Vector2f> GetSafeSpawnPoints()
        {
            // Find spawn points in map
            var spawnPoints = new List<Vector2f>();
            for (int i = 0; i < gameModel.CurrentMap.MapLayers[0].Length; i++)
            {
                if (gameModel.CurrentMap.MapLayers[0][i] == 0)
                {
                    // Check if its not outside the map
                    if (i % gameModel.CurrentMap.Width != 0 && i % gameModel.CurrentMap.Width != gameModel.CurrentMap.Width - 1)
                    {
                        // Check if its not on the edge of the map
                        if (i > gameModel.CurrentMap.Width && i < gameModel.CurrentMap.MapLayers[0].Length - gameModel.CurrentMap.Width - 1)
                        {
                            int x = i % (int)gameModel.CurrentMap.Width;
                            int y = i / (int)gameModel.CurrentMap.Width;

                            if (x > gameModel.CurrentMap.TileWidth && y > gameModel.CurrentMap.TileHeight)
                            { 
                                spawnPoints.Add(new Vector2f(x * gameModel.CurrentMap.TileWidth, y * gameModel.CurrentMap.TileHeight));
                            }
                        }
                    }
                }
            }

            return spawnPoints;
        }

        public void CreateMaps()
        {
            List<GateModel> gates = new List<GateModel>();
            GateModel shopGate = new GateModel();
            shopGate.GateSprite = new Sprite();
            shopGate.GateSprite.Position = new Vector2f(173, 20);
            shopGate.Hitbox = new RectangleShape();
            shopGate.InteractArea = new RectangleShape();
            shopGate.GateState = GateState.InShop;
            shopGate.IsGateReady = true;

            shopGate.GateTexts = new List<Text>();
            Text shopGateNameText = new Text();
            shopGateNameText.DisplayedString = "Shop gate";
            shopGateNameText.FillColor = Color.Red;
            shopGateNameText.CharacterSize = 28;
            shopGateNameText.OutlineColor = Color.Black;
            shopGateNameText.OutlineThickness = 2;
            shopGate.GateTexts.Add(shopGateNameText);

            Text shopGateMsgText = new Text();
            shopGateMsgText.DisplayedString = "Press E to enter the shop!";
            shopGateMsgText.CharacterSize = 28;
            shopGateMsgText.OutlineThickness = 2;
            shopGateMsgText.OutlineColor = Color.Black;
            shopGate.GateTexts.Add(shopGateMsgText);

            GateModel killArenaGate = new GateModel();
            killArenaGate.GateSprite = new Sprite();
            killArenaGate.GateSprite.Position = new Vector2f(900, 20);
            killArenaGate.Hitbox = new RectangleShape();
            killArenaGate.InteractArea = new RectangleShape();
            killArenaGate.GateState = GateState.InKillArena;
            killArenaGate.IsGateReady = true;

            killArenaGate.GateTexts = new List<Text>();
            Text killArenaGateNameText = new Text();
            killArenaGateNameText.DisplayedString = "Kill arena gate";
            killArenaGateNameText.FillColor = Color.Red;
            killArenaGateNameText.CharacterSize = 28;
            killArenaGateNameText.OutlineColor = Color.Black;
            killArenaGateNameText.OutlineThickness = 2;
            killArenaGate.GateTexts.Add(killArenaGateNameText);

            Text killArenaGateMsgText = new Text();
            killArenaGateMsgText.DisplayedString = "Come in to fight against enemies!";
            killArenaGateMsgText.CharacterSize = 28;
            killArenaGateMsgText.OutlineThickness = 2;
            killArenaGateMsgText.OutlineColor = Color.Black;
            killArenaGate.GateTexts.Add(killArenaGateMsgText);

            GateModel bossArenaGate = new GateModel();
            bossArenaGate.GateSprite = new Sprite();
            bossArenaGate.GateSprite.Position = new Vector2f(1650, 20);
            bossArenaGate.Hitbox = new RectangleShape();
            bossArenaGate.InteractArea = new RectangleShape();
            bossArenaGate.GateState = GateState.InBossArena;
            bossArenaGate.IsGateReady = true;

            bossArenaGate.GateTexts = new List<Text>();
            Text bossArenaGateNameText = new Text();
            bossArenaGateNameText.DisplayedString = "Boss arena gate";
            bossArenaGateNameText.FillColor = Color.Red;
            bossArenaGateNameText.CharacterSize = 28;
            bossArenaGateNameText.OutlineColor = Color.Black;
            bossArenaGateNameText.OutlineThickness = 2;
            bossArenaGate.GateTexts.Add(bossArenaGateNameText);

            Text bossArenaGateMsgText = new Text();
            bossArenaGateMsgText.DisplayedString = "Come in to fight against the \nboss, you can't come back!\nRequired XP level: 50";
            bossArenaGateMsgText.CharacterSize = 28;
            bossArenaGateMsgText.OutlineColor = Color.Black;
            bossArenaGateMsgText.OutlineThickness = 2;
            bossArenaGate.GateTexts.Add(bossArenaGateMsgText);

            GateModel backToLobbyGate = new GateModel();
            backToLobbyGate.GateSprite = new Sprite();
            backToLobbyGate.GateSprite.Position = new Vector2f(120, 20);
            backToLobbyGate.Hitbox = new RectangleShape();
            backToLobbyGate.InteractArea = new RectangleShape();
            backToLobbyGate.GateState = GateState.InLobby;
            backToLobbyGate.IsGateReady = false;

            backToLobbyGate.GateTexts = new List<Text>();
            Text backToLobbyGateNameText = new Text();
            backToLobbyGateNameText.DisplayedString = "Lobby gate";
            backToLobbyGateNameText.FillColor = Color.Red;
            backToLobbyGateNameText.CharacterSize = 28;
            backToLobbyGateNameText.OutlineColor = Color.Black;
            backToLobbyGateNameText.OutlineThickness = 2;
            backToLobbyGate.GateTexts.Add(backToLobbyGateNameText);

            Text backToLobbyGateMsgText = new Text();
            backToLobbyGateMsgText.DisplayedString = "Come back to lobby!";
            backToLobbyGateMsgText.CharacterSize = 28;
            backToLobbyGateMsgText.OutlineColor = Color.Black;
            backToLobbyGateMsgText.OutlineThickness = 2;
            backToLobbyGate.GateTexts.Add(backToLobbyGateMsgText);

            gates.Add(shopGate);
            gates.Add(killArenaGate);
            gates.Add(bossArenaGate);
            gates.Add(backToLobbyGate);

            gameModel.Gates = gates;

            // Texts
            gameModel.CreatorTexts = new List<Text>();
            Text creatorTitle = new Text();
            creatorTitle.DisplayedString = "Creators:";
            creatorTitle.FillColor = Color.Red;
            creatorTitle.CharacterSize = 80;
            creatorTitle.Position = new Vector2f(100, 950);
            creatorTitle.OutlineColor = Color.Black;
            creatorTitle.OutlineThickness = 2;
            gameModel.CreatorTexts.Add(creatorTitle);

            Text creatorName1 = new Text();
            creatorName1.DisplayedString = "Tóth Balázs - MWZX0D";
            creatorName1.FillColor = Color.White;
            creatorName1.CharacterSize = 40;
            creatorName1.Position = new Vector2f(100, 1050);
            creatorName1.OutlineColor = Color.Black;
            creatorName1.OutlineThickness = 2;
            gameModel.CreatorTexts.Add(creatorName1);

            Text creatorName2 = new Text();
            creatorName2.DisplayedString = "Horváth Zsolt - PBVGD1";
            creatorName2.FillColor = Color.White;
            creatorName2.CharacterSize = 40;
            creatorName2.Position = new Vector2f(100, 1100);
            creatorName2.OutlineThickness = 2;
            creatorName2.OutlineColor = Color.Black;
            gameModel.CreatorTexts.Add(creatorName2);

            Text creatorName3 = new Text();
            creatorName3.DisplayedString = "Ecseki Tamás - MAFWZU";
            creatorName3.FillColor = Color.White;
            creatorName3.CharacterSize = 40;
            creatorName3.Position = new Vector2f(100, 1150);
            creatorName3.OutlineColor = Color.Black;
            creatorName3.OutlineThickness = 2;
            gameModel.CreatorTexts.Add(creatorName3);

            gameModel.SettingsTexts = new List<Text>();
            Text settingsTitle = new Text();
            settingsTitle.DisplayedString = "Hotkeys:";
            settingsTitle.FillColor = Color.Red;
            settingsTitle.CharacterSize = 80;
            settingsTitle.Position = new Vector2f(1000, 950);
            settingsTitle.OutlineColor = Color.Black;
            settingsTitle.OutlineThickness = 2;
            gameModel.SettingsTexts.Add(settingsTitle);

            Text settingsPlayerMove = new Text();
            settingsPlayerMove.DisplayedString = "Moving: W A S D";
            settingsPlayerMove.FillColor = Color.White;
            settingsPlayerMove.CharacterSize = 40;
            settingsPlayerMove.Position = new Vector2f(1000, 1050);
            settingsPlayerMove.OutlineColor = Color.Black;
            settingsPlayerMove.OutlineThickness = 2;
            gameModel.SettingsTexts.Add(settingsPlayerMove);

            Text settingsPlayerShoot = new Text();
            settingsPlayerShoot.DisplayedString = "Shooting: Left click";
            settingsPlayerShoot.FillColor = Color.White;
            settingsPlayerShoot.CharacterSize = 40;
            settingsPlayerShoot.Position = new Vector2f(1000, 1100);
            settingsPlayerShoot.OutlineColor = Color.Black;
            settingsPlayerShoot.OutlineThickness = 2;
            gameModel.SettingsTexts.Add(settingsPlayerShoot);

            Text settingsPlayerReload = new Text();
            settingsPlayerReload.DisplayedString = "Reloading: R";
            settingsPlayerReload.FillColor = Color.White;
            settingsPlayerReload.CharacterSize = 40;
            settingsPlayerReload.Position = new Vector2f(1000, 1150);
            settingsPlayerReload.OutlineColor = Color.Black;
            settingsPlayerReload.OutlineThickness = 2;
            gameModel.SettingsTexts.Add(settingsPlayerReload);

            Text settingsPlayerSwitchWeapon = new Text();
            settingsPlayerSwitchWeapon.DisplayedString = "Switching weapon: Mouse wheel";
            settingsPlayerSwitchWeapon.FillColor = Color.White;
            settingsPlayerSwitchWeapon.CharacterSize = 40;
            settingsPlayerSwitchWeapon.Position = new Vector2f(1000, 1200);
            settingsPlayerSwitchWeapon.OutlineColor = Color.Black;
            settingsPlayerSwitchWeapon.OutlineThickness = 2;
            gameModel.SettingsTexts.Add(settingsPlayerSwitchWeapon);


            Text inventory = new Text();
            inventory.DisplayedString = "Inventory: I";
            inventory.FillColor = Color.White;
            inventory.CharacterSize = 40;
            inventory.Position = new Vector2f(1500, 1050);
            inventory.OutlineColor = Color.Black;
            inventory.OutlineThickness = 2;
            gameModel.SettingsTexts.Add(inventory);
        }

        public void PlayGameOverSound()
        {
            if (gameModel.GameOverSound.Status == SoundStatus.Stopped)
            {
                gameModel.GameOverSound.Volume = 70;
                gameModel.GameOverSound.Play();
            }
        }

        public void PlayGameWonSound()
        {
            if (gameModel.GameWonSound.Status == SoundStatus.Stopped)
            {
                gameModel.GameWonSound.Volume = 70;
                gameModel.GameWonSound.Play();
            }
        }
    }
}
