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

            gameModel.Songs = new List<Music>();
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic1.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic2.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic3.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic4.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic5.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic6.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic7.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic8.ogg"));
            gameModel.Songs.Add(new Music("Assets/Sounds/gameplaymusic9.ogg"));

            Music();

            var spawnPoints = GetSafeSpawnPoints();
            Random rnd = new Random();
            int spawnPointIndex = rnd.Next(0, spawnPoints.Count);
            gameModel.Player.Position = spawnPoints[spawnPointIndex];
        }

        public void SetTilemap(string tmxFile, string tilesetFile)
        {
            //var tilemapModel = tilemapRepository.LoadTMXFile(tmxFile);
            //TilemapLoader tmapLoader = new TilemapLoader(tilesetFile);
            //tmapLoader.LoadTMXFile(tmxFile);
            //tmapLoader.InitializeVertices();

            //gameModel.Map.CollidableIDs = new List<int>();
            //gameModel.Map.CollidableIDs.Add(4);
            //gameModel.Map.TilesetTexture = new Texture(tilesetFile);
            //gameModel.Map.Vertices = tmapLoader.Vertices;
            //gameModel.Map.MapLayers = tmapLoader.MapLayers;
            //gameModel.Map.Width = tmapLoader.Width;
            //gameModel.Map.Height = tmapLoader.Height;
            //gameModel.Map.TileWidth = tmapLoader.TileWidth;
            //gameModel.Map.TileHeight = tmapLoader.TileHeight;
            //gameModel.Map.Size = new Vector2u(tmapLoader.Width, tmapLoader.Height);
            //gameModel.Map.TileSize = new Vector2u(tmapLoader.TileWidth, tmapLoader.TileHeight);

            gameModel.Map.CollidableIDs = new List<int>();
            gameModel.Map.CollidableIDs.Add(4);
            gameModel.Map.TilesetTexture = new Texture(tilesetFile);

            // Kill Arena map
            uint killArenaWidth = 100;
            uint killArenaHeight = 100;
            float killArenaScale = 1f;
            uint killArenaTileWidth = 32;
            uint killArenaTileHeight = 32;
            int[] collisionLayer = tilemapLogic.MapGeneration(killArenaHeight, killArenaWidth, killArenaScale);
            gameModel.KillArenaMap.CollidableIDs = new List<int>();
            gameModel.KillArenaMap.CollidableIDs.Add(4);
            gameModel.KillArenaMap.TilesetTexture = new Texture(tilesetFile);
            gameModel.KillArenaMap.MapLayers = new List<int[]>();
            gameModel.KillArenaMap.MapLayers.Add(collisionLayer);
            gameModel.KillArenaMap.MapLayers.Add(collisionLayer);
            gameModel.KillArenaMap.Width = killArenaWidth;
            gameModel.KillArenaMap.Height = killArenaHeight;
            gameModel.KillArenaMap.TileWidth = killArenaTileWidth;
            gameModel.KillArenaMap.TileHeight = killArenaTileHeight;
            gameModel.KillArenaMap.Size = new Vector2u(killArenaWidth, killArenaHeight);
            gameModel.KillArenaMap.TileSize = new Vector2u(killArenaTileWidth, killArenaHeight);

            tilemapLogic.InitializeVertices(gameModel.KillArenaMap);

            gameModel.Map.Vertices = gameModel.KillArenaMap.Vertices;
            gameModel.Map.MapLayers = gameModel.KillArenaMap.MapLayers;
            gameModel.Map.Width = gameModel.KillArenaMap.Width;
            gameModel.Map.Height = gameModel.KillArenaMap.Height;
            gameModel.Map.TileWidth = gameModel.KillArenaMap.TileWidth;
            gameModel.Map.TileHeight = gameModel.KillArenaMap.TileHeight;
            gameModel.Map.Size = new Vector2u(gameModel.KillArenaMap.Width, gameModel.KillArenaMap.Height);
            gameModel.Map.TileSize = new Vector2u(gameModel.KillArenaMap.TileWidth, gameModel.KillArenaMap.TileHeight);
        }

        public void UpdatePlayer(RenderWindow window)
        {
            gameModel.Player.Hitbox.Size = new Vector2f(gameModel.Player.GetGlobalBounds().Width - 2f, gameModel.Player.GetGlobalBounds().Height - 2f);
            gameModel.Player.Hitbox.Position = new Vector2f(gameModel.Player.Position.X, gameModel.Player.Position.Y);
            gameModel.Player.Hitbox.Origin = new Vector2f(gameModel.Player.Origin.X, gameModel.Player.Origin.Y);

            playerLogic.UpdateAnimationTextures();
            playerLogic.UpdateWorldPositionByMouse(window);
            playerLogic.UpdateDeltaTime(deltaTime);
            playerLogic.UpdateTilePosition(gameModel.Map);
            playerLogic.HandleMapCollision(gameModel.Map);
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
            enemyLogic.UpdateAnimationTextures();
            enemyLogic.HandleBulletCollision();

            for (int i = 0; i < gameModel.Enemies.Count; i++)
            {
                gameModel.Enemies[i].Hitbox.Size = new Vector2f(gameModel.Enemies[i].GetGlobalBounds().Width - 1f, gameModel.Enemies[i].GetGlobalBounds().Height - 1f);
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

        public void UpdateTilemap()
        {
            tilemapLogic.UpdateItemAnimationTextures();
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

        public void SetView(ref View view, Vector2f size, Vector2f? center = null, FloatRect? viewport = null)
        {
            view = new View();
            view.Size = size;

            if (center != null)
            {
                view.Center = center.Value;
            }

            if (viewport != null)
            {
                view.Viewport = viewport.Value;
            }
        }

        public void CreateItems()
        {
            gameModel.CollectibleItems = new List<ICollectibleItem>();

            for (int i = 0; i < new Random().Next(30, 80); i++)
            {
                CollectibleItemModel coinItem = new CollectibleItemModel();
                coinItem.Item = new Sprite();
                coinItem.Item.Position = new Vector2f(new Random().Next() % gameModel.Map.GetMapWidth, new Random().Next() % gameModel.Map.GetMapHeight);
                coinItem.ItemType = Model.Game.Enums.ItemType.Coin;
                coinItem.CoinSoundBuffer = new SoundBuffer("Assets/Sounds/coin.ogg");
                coinItem.CoinSound = new Sound(coinItem.CoinSoundBuffer);
                coinItem.Id = (int)coinItem.ItemType;
                
                gameModel.CollectibleItems.Add(coinItem);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                    {
                        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % gameModel.Map.GetMapWidth, new Random().Next() % gameModel.Map.GetMapHeight);
                        j = 0;
                    }
                }
            }

            for (int i = 0; i < new Random().Next(15, 40); i++)
            {
                CollectibleItemModel healtPotionItem = new CollectibleItemModel();
                healtPotionItem.Item = new Sprite();
                healtPotionItem.Item.Position = new Vector2f(new Random().Next() % gameModel.Map.GetMapWidth, new Random().Next() % gameModel.Map.GetMapHeight);
                healtPotionItem.ItemType = Model.Game.Enums.ItemType.Health_Potion;
                healtPotionItem.Id = (int)healtPotionItem.ItemType;
                healtPotionItem.IconFileName = "health_potion.png";
                
                gameModel.CollectibleItems.Add(healtPotionItem);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                    {
                        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % gameModel.Map.GetMapWidth, new Random().Next() % gameModel.Map.GetMapHeight);
                        j = 0;
                    }
                }
            }

            for (int i = 0; i < new Random().Next(15, 40); i++)
            {
                CollectibleItemModel speedPotion = new CollectibleItemModel();
                speedPotion.Item = new Sprite();
                speedPotion.Item.Position = new Vector2f(new Random().Next() % gameModel.Map.GetMapWidth, new Random().Next() % gameModel.Map.GetMapHeight);
                speedPotion.ItemType = Model.Game.Enums.ItemType.Speed_Potion;
                speedPotion.Id = (int)speedPotion.ItemType;
                speedPotion.IconFileName = "speed_potion.png";

                gameModel.CollectibleItems.Add(speedPotion);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                    {
                        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % gameModel.Map.GetMapWidth, new Random().Next() % gameModel.Map.GetMapHeight);
                        j = 0;
                    }
                }
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
                        var tilePosition = new Vector2i((int)((int)xTilePosition / gameModel.Map.TileSize.X), (int)((int)yTilePosition / gameModel.Map.TileSize.Y)) + new Vector2i(x, y);
                        var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                        if (gameModel.Map.CollidableIDs.Contains(currentTileID) == false)
                        {
                            continue;
                        }

                        var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                        var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.Map.TileSize.X, gameModel.Map.TileSize.Y);
                        var rect = item.Item.GetGlobalBounds();

                        if (tileRect.Intersects(rect))
                        {
                            gameModel.CollectibleItems.Remove(item);

                            var optimalPosition = new Vector2f(); 
                            var optimalDistance = float.MaxValue;

                            for (int xP = 0; xP < gameModel.Map.Size.X; xP++)
                            {
                                for (int yP = 0; yP < gameModel.Map.Size.Y; yP++)
                                {
                                    var tileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, xP, yP);
                                    if (gameModel.Map.CollidableIDs.Contains(tileID) == false)
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
                if (item.Item.Position.X < 0 || item.Item.Position.X > gameModel.Map.Size.X * gameModel.Map.TileSize.X || item.Item.Position.Y < 0 || item.Item.Position.Y > gameModel.Map.Size.Y * gameModel.Map.TileSize.Y)
                {
                    gameModel.CollectibleItems.Remove(item);
                }
            }
        }
        
        public void Music()
        {

            //foreach (var music in gameModel.Songs)
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

            if (gameModel.Songs[0].Status == SoundStatus.Stopped)
            {
                gameModel.Songs[0].Volume = 25;
                gameModel.Songs[0].Play();
            }
        }

        public void CameraEdges()
        {
            if (gameModel.CameraView.Center.X < gameModel.CameraView.Size.X / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Size.X / 2f, gameModel.CameraView.Center.Y);
            }
            if (gameModel.CameraView.Center.X > gameModel.Map.GetMapWidth - gameModel.CameraView.Size.X / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.Map.GetMapWidth - gameModel.CameraView.Size.X / 2f, gameModel.CameraView.Center.Y);
            }

            if (gameModel.CameraView.Center.Y < gameModel.CameraView.Size.Y / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Center.X, gameModel.CameraView.Size.Y / 2f);
            }
            if (gameModel.CameraView.Center.Y > gameModel.Map.GetMapHeight - gameModel.CameraView.Size.Y / 2f)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Center.X, gameModel.Map.GetMapWidth - gameModel.CameraView.Size.Y / 2f);
            }
        }

        public List<Vector2f> GetSafeSpawnPoints()
        {
            // Find spawn points in map
            var spawnPoints = new List<Vector2f>();
            for (int i = 0; i < gameModel.Map.MapLayers[0].Length; i++)
            {
                if (gameModel.Map.MapLayers[0][i] == 0)
                {
                    // Check if its not outside the map
                    if (i % gameModel.Map.Width != 0 && i % gameModel.Map.Width != gameModel.Map.Width - 5)
                    {
                        // Check if its not on the edge of the map
                        if (i > gameModel.Map.Width && i < gameModel.Map.MapLayers[0].Length - gameModel.Map.Width - 5)
                        {
                            // Check if its not on the edge of the map
                            //spawnPoints.Add(new Vector2f((i % gameModel.Map.Width) * gameModel.Map.TileWidth, (i / gameModel.Map.Width) * gameModel.Map.TileHeight));

                            int x = i % (int)gameModel.Map.Width;
                            int y = i / (int)gameModel.Map.Width;
                            spawnPoints.Add(new Vector2f(x * gameModel.Map.TileWidth, y * gameModel.Map.TileHeight));
                        }
                    }
                }
            }

            return spawnPoints;
        }
    }
}
