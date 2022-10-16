using Logic.Game.Interfaces;
using Model;
using Model.Game;
using Model.Game.Classes;
using Model.Game.Enums;
using Model.Game.Interfaces;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

            gameModel.Enemies = new List<EnemyModel>();
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

            
            SetTilemap("map.tmx", "tilemap.png");
            CreateSpawnableItems();
            CreateSpawnableEnemies();
            //SpawnItems();

            gameModel.Musics = new List<Music>();
            gameModel.Musics.Add(new Music("Assets/Sounds/motionless.ogg"));
            gameModel.Musics.Add(new Music("Assets/Sounds/bullet.ogg"));
        }

        public void SetTilemap(string tmxFile, string tilesetFile)
        {
            TilemapLoader tmapLoader = new TilemapLoader(tilesetFile);
            tmapLoader.LoadTMXFile(tmxFile);
            tmapLoader.InitializeVertices();

            gameModel.Map.CollidableIDs = new List<int>();
            gameModel.Map.CollidableIDs.Add(4);
            gameModel.Map.TilesetTexture = new Texture(tilesetFile);
            gameModel.Map.Vertices = tmapLoader.Vertices;
            gameModel.Map.MapLayers = tmapLoader.MapLayers;
            gameModel.Map.Width = tmapLoader.Width;
            gameModel.Map.Height = tmapLoader.Height;
            gameModel.Map.TileWidth = tmapLoader.TileWidth;
            gameModel.Map.TileHeight = tmapLoader.TileHeight;
            gameModel.Map.Size = new Vector2u(tmapLoader.Width, tmapLoader.Height);
            gameModel.Map.TileSize = new Vector2u(tmapLoader.TileWidth, tmapLoader.TileHeight);
        }

        public void UpdatePlayer(RenderWindow window)
        {
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
        }
        
        public void UpdateBullets(RenderWindow window)
        {
            bulletLogic.HandleMapCollision(window);

            foreach (ObjectEntityModel chest in gameModel.Objects)
            {
                bulletLogic.HandleObjectCollision(chest);
            }

            bulletLogic.Update();
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

        public void CreateSpawnableItems()
        {
            gameModel.CollectibleItems = new List<ICollectibleItem>();

            for (int i = 0; i < new Random().Next(5, 30); i++)
            {
                CollectibleItemModel coinItem = new CollectibleItemModel();
                coinItem.Item = new Sprite();
                coinItem.Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                coinItem.ItemType = Model.Game.Enums.ItemType.Coin;
                coinItem.Id = (int)coinItem.ItemType;
                
                gameModel.CollectibleItems.Add(coinItem);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                    {
                        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                        j = 0;
                    }
                }
            }

            for (int i = 0; i < new Random().Next(1, 5); i++)
            {
                CollectibleItemModel healtPotionItem = new CollectibleItemModel();
                healtPotionItem.Item = new Sprite();
                healtPotionItem.Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                healtPotionItem.ItemType = Model.Game.Enums.ItemType.Health_Potion;
                healtPotionItem.Id = (int)healtPotionItem.ItemType;
                
                gameModel.CollectibleItems.Add(healtPotionItem);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                    {
                        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                        j = 0;
                    }
                }
            }

            for (int i = 0; i < new Random().Next(1, 5); i++)
            {
                CollectibleItemModel speedPotion = new CollectibleItemModel();
                speedPotion.Item = new Sprite();
                speedPotion.Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                speedPotion.ItemType = Model.Game.Enums.ItemType.Speed_Potion;
                speedPotion.Id = (int)speedPotion.ItemType;
                
                gameModel.CollectibleItems.Add(speedPotion);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.CollectibleItems[i].Item.GetGlobalBounds().Intersects(gameModel.CollectibleItems[j].Item.GetGlobalBounds()))
                    {
                        gameModel.CollectibleItems[i].Item.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
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

        public void CreateSpawnableEnemies()
        {
            for (int i = 0; i < new Random().Next(2, 6); i++)
            {
                EnemyModel enemy = new EnemyModel();
                enemy.Position = new Vector2f(new Random().Next() % 600, new Random().Next() % 600);
                enemy.Speed = 30f;
                enemy.EnemyType = Model.Game.Enums.EnemyType.Basic;

                gameModel.Enemies.Add(enemy);
                for (int j = 0; j < i - 1; j++)
                {
                    if (gameModel.Enemies[i].GetGlobalBounds().Intersects(gameModel.Enemies[j].GetGlobalBounds()))
                    {
                        gameModel.Enemies.RemoveAt(i);
                        j = 0;
                    }
                }
            }
        }

        public void SpawnEnemies()
        {
            foreach (var enemy in gameModel.Enemies)
            {
                for (int y = -3; y < 3; y++)
                {
                    for (int x = -3; x < 3; x++)
                    {
                        var xTilePosition = enemy.Position.X;
                        var yTilePosition = enemy.Position.Y;
                        var tilePosition = new Vector2i((int)((int)xTilePosition / gameModel.Map.TileSize.X), (int)((int)yTilePosition / gameModel.Map.TileSize.Y)) + new Vector2i(x, y);
                        var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, tilePosition.X, tilePosition.Y);
                        if (gameModel.Map.CollidableIDs.Contains(currentTileID) == false)
                        {
                            continue;
                        }

                        var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(tilePosition.X, tilePosition.Y);
                        var tileRect = new FloatRect(currentTileWorldPosition.X, currentTileWorldPosition.Y, gameModel.Map.TileSize.X, gameModel.Map.TileSize.Y);
                        var rect = enemy.GetGlobalBounds();

                        if (tileRect.Intersects(rect))
                        {
                            gameModel.Enemies.Remove(enemy);

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

                            enemy.Position = optimalPosition;
                            gameModel.Enemies.Add(enemy);

                            return;
                        }
                    }
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
            foreach (var music in gameModel.Musics)
            {
                //if (k > 1)
                //{
                //    music.Stop();
                //    gameModel.Musics[k - 1].Play();
                //}
                //else
                //{
                //    music.Play();
                //}

                //if (music.Status == SFML.Audio.SoundStatus.Stopped)
                //{
                //    music.Volume = 30;
                //    music.Play();
                //}
            }
        }
    }
}
