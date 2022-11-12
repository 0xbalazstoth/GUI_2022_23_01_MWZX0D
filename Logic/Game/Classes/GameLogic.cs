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
        private IObjectEntityLogic objectEntityLogic;
        private IBulletLogic bulletLogic;

        private Clock deltaTimeClock;
        private float deltaTime;

        public Clock GetDeltaTimeClock { get => deltaTimeClock; }
        public float GetDeltaTime { get => deltaTime; }

        public GameLogic(IGameModel gameModel, ITilemapLogic tilemapLogic, IPlayerLogic playerLogic, IEnemyLogic enemyLogic, IObjectEntityLogic objectEntityLogic, IBulletLogic bulletLogic)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;
            this.playerLogic = playerLogic;
            this.enemyLogic = enemyLogic;
            this.objectEntityLogic = objectEntityLogic;
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
            enemyLogic.CreateEnemies();

            //gameModel.Musics = new List<Music>();
            //gameModel.Musics.Add(new Music("Assets/Sounds/motionless.ogg"));
            //gameModel.Musics.Add(new Music("Assets/Sounds/bullet.ogg"));

            Music();

            var spawnPoints = GetSafeSpawnPoints();
            Random rnd = new Random();
            int spawnPointIndex = rnd.Next(0, spawnPoints.Count);
            //gameModel.Player.Position = spawnPoints[spawnPointIndex];
            
            gameModel.Player.Position = new Vector2f(300, 500);
            gameModel.Player.PlayerState = GateState.InLobby;

            List<GateModel> gates = new List<GateModel>();
            GateModel shopGate = new GateModel();
            shopGate.GateSprite = new Sprite();
            shopGate.GateSprite.Position = new Vector2f(173, 20);
            shopGate.Hitbox = new RectangleShape();
            shopGate.InteractArea = new RectangleShape();
            shopGate.GateState = GateState.InShop;

            GateModel killArenaGate = new GateModel();
            killArenaGate.GateSprite = new Sprite();
            killArenaGate.GateSprite.Position = new Vector2f(900, 20);
            killArenaGate.Hitbox = new RectangleShape();
            killArenaGate.InteractArea = new RectangleShape();
            killArenaGate.GateState = GateState.InKillArena;

            GateModel bossArenaGate = new GateModel();
            bossArenaGate.GateSprite = new Sprite();
            bossArenaGate.GateSprite.Position = new Vector2f(1650, 20);
            bossArenaGate.Hitbox = new RectangleShape();
            bossArenaGate.InteractArea = new RectangleShape();
            bossArenaGate.GateState = GateState.InBossArena;

            gates.Add(shopGate);
            gates.Add(killArenaGate);
            gates.Add(bossArenaGate);

            gameModel.Gates = gates;
        }

        public void SetTilemap(string tmxFile, string tilesetFile)
        {
            gameModel.CurrentMap.CollidableIDs = new List<int>();
            gameModel.CurrentMap.TilesetTexture = new Texture(tilesetFile);
            foreach (var id in gameModel.MapCollidibleIDs)
            {
                gameModel.CurrentMap.CollidableIDs.Add(id);
            }

            #region Lobby
            ManualTilemapLoadingHandler tilemapLoader = new ManualTilemapLoadingHandler();
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
            uint killArenaWidth = 100;
            uint killArenaHeight = 100;
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
            playerLogic.HandleInventory();
            playerLogic.UpdateHP();
            playerLogic.UpdateSpeedPotionTimer();
            playerLogic.HandleEnemyBulletCollision();
            playerLogic.HandleGateCollision();
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
                }

                enemyLogic.SpawnEnemies(deltaTime);

                for (int i = 0; i < gameModel.Enemies.Count; i++)
                {
                    gameModel.Enemies[i].Gun.Scale = new Vector2f(2.5f, 2.5f);
                    gameModel.Enemies[i].Gun.Origin = new Vector2f(gameModel.Enemies[i].Gun.Texture.Size.X / 2f, gameModel.Enemies[i].Gun.Texture.Size.Y / 2f);
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
            else if (gameModel.Player.PlayerState == Model.Game.Enums.GateState.InLobby || gameModel.Player.PlayerState == GateState.InShop)
            {
                for (int i = 0; i < gameModel.Gates.Count; i++)
                {
                    gameModel.Gates[i].Hitbox.Size = new Vector2f(gameModel.Gates[i].GateSprite.GetGlobalBounds().Width, gameModel.Gates[i].GateSprite.GetGlobalBounds().Height - 95f);
                    gameModel.Gates[i].Hitbox.Position = new Vector2f(gameModel.Gates[i].GateSprite.Position.X, gameModel.Gates[i].GateSprite.Position.Y + 30f);
                    gameModel.Gates[i].Hitbox.Origin = new Vector2f(gameModel.Gates[i].GateSprite.Origin.X, gameModel.Gates[i].GateSprite.Origin.Y);

                    gameModel.Gates[i].InteractArea.Size = new Vector2f(gameModel.Gates[i].GateSprite.GetGlobalBounds().Width, 50f);
                    gameModel.Gates[i].InteractArea.Position = new Vector2f(gameModel.Gates[i].GateSprite.Position.X, gameModel.Gates[i].GateSprite.GetGlobalBounds().Height - 32f);

                    gameModel.Gates[i].GateSprite.Texture = gameModel.Gates[i].Animations[0].Texture;
                    gameModel.Gates[i].GateSprite.TextureRect = gameModel.Gates[i].Animations[0].TextureRect;
                }
            }
        }

        public void UpdateDeltaTime()
        {
            deltaTime = deltaTimeClock.ElapsedTime.AsSeconds();
            deltaTimeClock.Restart();
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

            for (int i = 0; i < new Random().Next(50, 130); i++)
            {
                CollectibleItemModel coinItem = new CollectibleItemModel();
                coinItem.Item = new Sprite();
                coinItem.Item.Position = new Vector2f(new Random().Next() % gameModel.KillArenaMap.GetMapWidth, new Random().Next() % gameModel.CurrentMap.GetMapHeight);
                coinItem.ItemType = Model.Game.Enums.ItemType.Coin;
                coinItem.CoinSoundBuffer = new SoundBuffer("Assets/Sounds/coin.ogg");
                coinItem.CoinSound = new Sound(coinItem.CoinSoundBuffer);
                coinItem.Id = (int)coinItem.ItemType;
                
                gameModel.CollectibleItems.Add(coinItem);
                //for (int j = 0; j < i - 1; j++)
                //{
                //    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                //    {
                //        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % gameModel.CurrentMap.GetMapWidth, new Random().Next() % gameModel.CurrentMap.GetMapHeight);
                //        j = 0;
                //    }
                //}
            }

            for (int i = 0; i < new Random().Next(30, 70); i++)
            {
                CollectibleItemModel healtPotionItem = new CollectibleItemModel();
                healtPotionItem.Item = new Sprite();
                healtPotionItem.Item.Position = new Vector2f(new Random().Next() % gameModel.KillArenaMap.GetMapWidth, new Random().Next() % gameModel.KillArenaMap.GetMapHeight);
                healtPotionItem.ItemType = Model.Game.Enums.ItemType.Health_Potion;
                healtPotionItem.Id = (int)healtPotionItem.ItemType;
                healtPotionItem.IconFileName = "health_potion.png";
                
                gameModel.CollectibleItems.Add(healtPotionItem);
                //for (int j = 0; j < i - 1; j++)
                //{
                //    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                //    {
                //        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % gameModel.CurrentMap.GetMapWidth, new Random().Next() % gameModel.CurrentMap.GetMapHeight);
                //        j = 0;
                //    }
                //}
            }

            for (int i = 0; i < new Random().Next(30, 70); i++)
            {
                CollectibleItemModel speedPotion = new CollectibleItemModel();
                speedPotion.Item = new Sprite();
                speedPotion.Item.Position = new Vector2f(new Random().Next() % gameModel.KillArenaMap.GetMapWidth, new Random().Next() % gameModel.KillArenaMap.GetMapHeight);
                speedPotion.ItemType = Model.Game.Enums.ItemType.Speed_Potion;
                speedPotion.Id = (int)speedPotion.ItemType;
                speedPotion.IconFileName = "speed_potion.png";

                gameModel.CollectibleItems.Add(speedPotion);
                //for (int j = 0; j < i - 1; j++)
                //{
                //    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                //    {
                //        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % gameModel.CurrentMap.GetMapWidth, new Random().Next() % gameModel.CurrentMap.GetMapHeight);
                //        j = 0;
                //    }
                //}
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
            //if (gameModel.Music == null)
            //{
            //    gameModel.Music = new Music("Resources/Music/BackgroundMusic.ogg");
            //    gameModel.Music.Loop = true;
            //    gameModel.Music.Volume = 50;
            //    gameModel.Music.Play();
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
    }
}
