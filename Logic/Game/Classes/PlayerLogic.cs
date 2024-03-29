﻿using Logic.Game.Classes;
using Logic.Game.Interfaces;
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
using System.Timers;

namespace Logic.Game.Classes
{
    public class PlayerLogic : IPlayerLogic
    {
        private IGameModel gameModel;
        private ITilemapLogic tilemapLogic;
        
        private Vector2f movementDirection;
        private Vector2f previousPosition;

        public PlayerLogic(IGameModel gameModel, ITilemapLogic tilemapLogic)
        {
            this.gameModel = gameModel;
            this.tilemapLogic = tilemapLogic;

            gameModel.Player = new PlayerModel();
            this.gameModel.Player.MaxSpeed = 180f;
            this.gameModel.Player.Speed = gameModel.Player.MaxSpeed;
            this.gameModel.Player.CurrentHP = this.gameModel.Player.MaxHP;
            
            this.gameModel.Player.Hitbox = new RectangleShape();

            gameModel.Player.Gun = gameModel.Guns[0]; // Default gun
            this.gameModel.Player.Gun.Bullets = new List<BulletModel>();
            
            previousPosition = this.gameModel.Player.Position;

            this.gameModel.Player.Inventory = new InventoryModel();
            this.gameModel.Player.Inventory.Items = new Dictionary<int, ICollectibleItem>();

            this.gameModel.Player.HPSprite = new Sprite();
            this.gameModel.Player.HPSprite.Position = new Vector2f(this.gameModel.Player.Position.X, this.gameModel.Player.Position.Y);

            this.gameModel.Player.HPText = new Text();
            this.gameModel.Player.HPText.Position = new Vector2f(this.gameModel.Player.Position.X, this.gameModel.Player.Position.Y);
            this.gameModel.Player.HPText.CharacterSize = 16;
            this.gameModel.Player.HPText.FillColor = Color.Red;
        }

        public Vector2f GetDirectionFromInput(Vector2f direction)
        {
            Vector2 numericsVector = Vector2.Normalize(new(direction.X, direction.Y));
            return new(numericsVector.X, numericsVector.Y);
        }

        public void UpdateTilePosition(TilemapModel tilemap)
        {
            var x = gameModel.Player.Position.X + gameModel.Player.Origin.X;
            var y = gameModel.Player.Position.Y + gameModel.Player.Origin.Y;

            gameModel.Player.TilePosition = new Vector2i((int)(x / tilemap.TileSize.X), (int)(y / tilemap.TileSize.Y));
        }

        public void HandleMovement(Vector2f direction)
        {
            var movementDirection = GetDirectionFromInput(direction);
            if (float.IsNaN(movementDirection.X) || float.IsNaN(movementDirection.Y))
                return;

            previousPosition = gameModel.Player.Position;
            gameModel.Player.Position += movementDirection * gameModel.Player.DeltaTime * gameModel.Player.Speed;
            this.movementDirection = movementDirection;
        }

        public void UpdateAnimationTextures()
        {
            gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.IdleRight].Texture;
            gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.IdleRight].TextureRect;
            gameModel.Player.Origin = new Vector2f(gameModel.Player.TextureRect.Width / 2, gameModel.Player.TextureRect.Height / 2);
        }

        public void UpdateDeltaTime(float dt)
        {
            gameModel.Player.DeltaTime = dt;
        }

        public void UpdateWorldPositionByMouse(RenderWindow window)
        {
            gameModel.Player.Center = new Vector2f(gameModel.Player.Position.X + gameModel.Player.GetGlobalBounds().Width / 2f, gameModel.Player.Position.Y + gameModel.Player.GetGlobalBounds().Height / 2f);
            gameModel.MousePositionWindow = (Vector2f)Mouse.GetPosition(window);
            gameModel.WorldPositionInCamera = window.MapPixelToCoords(new Vector2i((int)gameModel.MousePositionWindow.X, (int)gameModel.MousePositionWindow.Y), gameModel.CameraView);

            gameModel.Player.AimDirection = gameModel.WorldPositionInCamera - gameModel.Player.Center;
            gameModel.Player.AimDirectionNormalized = gameModel.Player.AimDirection / (float)Math.Sqrt(gameModel.Player.AimDirection.X * gameModel.Player.AimDirection.X + gameModel.Player.AimDirection.Y * gameModel.Player.AimDirection.Y);
        }

        public void HandleEnemyCollision()
        {
            foreach (var enemy in gameModel.Enemies)
            {
                if (gameModel.Player.GetGlobalBounds().Intersects(enemy.GetGlobalBounds()))
                {
                    //gameModel.Player.Position = previousPosition;
                }
            }
        }

        public void HandleObjectCollision(Sprite item)
        {
            if (gameModel.Player.GetGlobalBounds().Intersects(item.GetGlobalBounds()))
            {
                gameModel.Player.Position = previousPosition;
            }
        }

        public void FlipAndRotateGun()
        {
            var angle = (float)Math.Atan2(gameModel.Player.AimDirectionNormalized.Y, gameModel.Player.AimDirectionNormalized.X) * 180f / (float)Math.PI;

            // Change player animation texture by aim direction
            if (angle > 45f && angle < 135f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Down].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Down].TextureRect;
            }
            else if (angle < -45f && angle > -135f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Up].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Up].TextureRect;
            }
            else if (angle > 135f || angle < -135f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Left].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Left].TextureRect;
            }
            else if (angle < 45f && angle > -45f)
            {
                gameModel.Player.Texture = gameModel.Player.Animations[MovementDirection.Right].Texture;
                gameModel.Player.TextureRect = gameModel.Player.Animations[MovementDirection.Right].TextureRect;
            }

            // Rotate gun
            if (angle > 90f || angle < -90f)
            {
                gameModel.Player.Gun.Scale = new Vector2f(-2.5f, 2.5f);
                gameModel.Player.Gun.Rotation = angle + 180f;
                gameModel.Player.Gun.Position = new Vector2f(gameModel.Player.Position.X - 10, gameModel.Player.Position.Y + 5);
            }
            else
            {
                gameModel.Player.Gun.Scale = new Vector2f(2.5f, 2.5f);
                gameModel.Player.Gun.Rotation = angle;
                gameModel.Player.Gun.Position = new Vector2f(gameModel.Player.Position.X + 10, gameModel.Player.Position.Y + 5);
            }
        }

        public void HandleMapCollision(TilemapModel tilemap)
        {
            if (gameModel.Player.TilePosition.X < 1 || gameModel.Player.TilePosition.X > tilemap.Size.X - 1 || gameModel.Player.TilePosition.Y < 2 || gameModel.Player.TilePosition.Y > tilemap.Size.Y - 0.2)
            {
                gameModel.Player.Position = previousPosition;
                return;
            }

            for (int y = -2; y < 2; y++)
            {
                for (int x = -2; x < 2; x++)
                {
                    var currentTilePosition = gameModel.Player.TilePosition + new Vector2i(x, y);
                    var currentTileID = tilemapLogic.GetTileID(TilemapLogic.COLLISION_LAYER, currentTilePosition.X, currentTilePosition.Y);
                    if (tilemap.CollidableIDs.Contains(currentTileID) == false)
                    {
                        continue;
                    }

                    var currentTileWorldPosition = tilemapLogic.GetTileWorldPosition(currentTilePosition.X, currentTilePosition.Y);
                    var tileRect = new FloatRect(currentTileWorldPosition, new(tilemap.TileSize.X, tilemap.TileSize.Y));
                    var rect = new FloatRect(gameModel.Player.GetGlobalBounds().Left, gameModel.Player.GetGlobalBounds().Top, gameModel.Player.Hitbox.Size.X, gameModel.Player.Hitbox.Size.Y);

                    if (rect.Intersects(tileRect))
                    {
                        gameModel.Player.Position = previousPosition;
                        return;
                    }
                }
            }
        }

        public void AddItemToInventory(ICollectibleItem item)
        {
            if (gameModel.Player.Inventory.Capacity < gameModel.Player.Inventory.MaxCapacity)
            {
                gameModel.Player.Inventory.Capacity++;
                if (gameModel.Player.Inventory.Items.ContainsKey(item.Id))
                {
                    gameModel.Player.Inventory.Items[item.Id].Quantity++;
                }
                else
                {
                    gameModel.Player.Inventory.Items.Add(item.Id, item);
                    gameModel.Player.Inventory.Items[item.Id].Quantity = 1;
                }
            }
            foreach (var inventoryItem in gameModel.Player.Inventory.Items)
            {
                Trace.WriteLine($"Id: {inventoryItem.Value.ItemType}, Quantity: {gameModel.Player.Inventory.Items[inventoryItem.Key].Quantity}");
            }
        }

        public void RemoveItemFromInventory(ICollectibleItem item)
        {
            if (gameModel.Player.Inventory.Capacity > 0)
            {
                gameModel.Player.Inventory.Capacity--;
                if (gameModel.Player.Inventory.Items.ContainsKey(item.Id))
                {
                    gameModel.Player.Inventory.Items[item.Id].Quantity--;
                    if (gameModel.Player.Inventory.Items[item.Id].Quantity <= 0)
                    {
                        gameModel.Player.Inventory.Items.Remove(item.Id);
                    }
                }
            }
            foreach (var inventoryItem in gameModel.Player.Inventory.Items)
            {
                Trace.WriteLine($"Id: {inventoryItem.Value.ItemType}, Quantity: {gameModel.Player.Inventory.Items[inventoryItem.Key].Quantity}");
            }
        }

        public void HandleInventory()
        {
            foreach (CollectibleItemModel item in gameModel.CollectibleItems)
            {
                if (gameModel.Player.GetGlobalBounds().Intersects(item.Item.GetGlobalBounds()))
                {
                    if (item.ItemType != Model.Game.Enums.ItemType.Coin)
                    {
                        var items = gameModel.Player.Inventory.Items.Sum(x => x.Value.Quantity);

                        if (items < gameModel.Player.Inventory.MaxCapacity)
                        {
                            Trace.WriteLine($"{item.ItemType} has been collected");
                            AddItemToInventory(item);
                            item.IsCollected = true;
                            if (item.IsCollected)
                            {
                                gameModel.CollectibleItems.Remove(item);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (item.ItemType == ItemType.Coin)
                        {
                            if (item.CoinSound.Status == SFML.Audio.SoundStatus.Stopped)
                            {
                                item.CoinSound.Volume = 30;
                                item.CoinSound.Play();
                            }

                            gameModel.Player.CurrentCoins++;
                            Trace.WriteLine($"Player current coins: {gameModel.Player.CurrentCoins}");
                        }

                        Trace.WriteLine($"{item.ItemType} has been collected");
                        item.IsCollected = true;
                        if (item.IsCollected)
                        {
                            gameModel.CollectibleItems.Remove(item);
                            return;
                        }
                    }
                } 
            }
        }

        public void ReloadGun()
        {
            if (gameModel.Player.Gun.CurrentAmmo == 0)
            {
                gameModel.Player.Gun.CurrentAmmo = gameModel.Player.Gun.MaxAmmo;

                if (gameModel.Player.Gun.ReloadSound.Status == SFML.Audio.SoundStatus.Stopped)
                {
                    gameModel.Player.Gun.ReloadSound.Play();
                }
            }

            else if (gameModel.Player.Gun.CurrentAmmo < gameModel.Player.Gun.MaxAmmo)
            {
                gameModel.Player.Gun.CurrentAmmo = gameModel.Player.Gun.MaxAmmo;

                if (gameModel.Player.Gun.ReloadSound.Status == SFML.Audio.SoundStatus.Stopped)
                {
                    gameModel.Player.Gun.ReloadSound.Play();
                }

                gameModel.Player.Gun.MaxAmmo = gameModel.Player.Gun.CurrentAmmo;
            }
        }

        public void UpdateHP()
        {
            gameModel.Player.HPSprite.Position = new Vector2f(gameModel.Player.Position.X - 16f, gameModel.Player.Position.Y - 50f);
            gameModel.Player.HPText.Position = new Vector2f(gameModel.Player.HPSprite.Position.X + 18f, gameModel.Player.HPSprite.Position.Y - (gameModel.Player.HPSprite.GetGlobalBounds().Height / 2f) + 4f);

            gameModel.Player.HPText.DisplayedString = $"{gameModel.Player.CurrentHP}";
        }

        public void UseItemFromInventory(ICollectibleItem item)
        {
            if (item.ItemType == ItemType.Health_Potion)
            {
               if (gameModel.Player.CurrentHP < gameModel.Player.MaxHP)
                {
                    // Increment player HP
                    // Check the difference, if its more than 10, calculate the difference and add it to the player HP
                    // If its less than 10, add 10 to the player HP
                    if (gameModel.Player.MaxHP - gameModel.Player.CurrentHP >= 10)
                    {
                        gameModel.Player.CurrentHP += 10;
                    }
                    else
                    {
                        gameModel.Player.CurrentHP += gameModel.Player.MaxHP - gameModel.Player.CurrentHP;
                    }

                    if (gameModel.HealthPotionSound.Status == SoundStatus.Stopped)
                    {
                        gameModel.HealthPotionSound.Volume = 50;
                        gameModel.HealthPotionSound.Play();
                    }

                    RemoveItemFromInventory(item);
                }
            }
            else if (item.ItemType == ItemType.Speed_Potion)
            {
                if (gameModel.Player.IsSpeedPotionIsInUse == false)
                {
                    gameModel.Player.IsSpeedPotionIsInUse = true;
                    gameModel.Player.LastPotionEffect = DateTime.Now;

                    if (gameModel.SpeedPotionSound.Status == SoundStatus.Stopped)
                    {
                        gameModel.SpeedPotionSound.Volume = 50;
                        gameModel.SpeedPotionSound.Play();
                    }

                    RemoveItemFromInventory(item);
                }
            }
        }
        
        public void UpdateSpeedPotionTimer()
        {
            if (DateTime.Now.Subtract(gameModel.Player.LastPotionEffect).TotalSeconds > 10)
            {
                gameModel.Player.Speed = gameModel.Player.MaxSpeed;
                gameModel.Player.IsSpeedPotionIsInUse = false;
            }
            else
            {
                gameModel.Player.Speed = 300f;
                gameModel.Player.IsSpeedPotionIsInUse = true;
            }
        }

        public void Shoot()
        {
            // Player can shoot every x seconds
            if (gameModel.Player.Gun.LastFired + gameModel.Player.Gun.FiringInterval < DateTime.Now)
            {
                // Check if player has ammo based on max ammo
                if (gameModel.Player.Gun.CurrentAmmo > 0 && (gameModel.Player.Gun.CurrentAmmo <= gameModel.Player.Gun.MaxAmmo))
                {
                    if (gameModel.Player.Gun.GunType == GunType.Shotgun)
                    {
                        CreateTemporaryShotgunBullet();
                        PushbackByRecoil(25f);
                    }
                    
                    if (gameModel.Player.Gun.GunType == GunType.Rifle)
                    {
                        CreateTemporaryRifleBullet();
                        PushbackByRecoil(10f);
                    }

                    gameModel.Player.Gun.CurrentAmmo--;
                    gameModel.Player.Gun.LastFired = DateTime.Now;
                    gameModel.Player.Gun.ShootSounds.Add(gameModel.Player.Gun.ShootSound);

                    foreach (var shootSound in gameModel.Player.Gun.ShootSounds)
                    {
                        shootSound.Play();

                        if (shootSound.Status == SoundStatus.Stopped)
                        {
                            gameModel.Player.Gun.ShootSounds.Remove(shootSound);
                            return;
                        }
                    }

                    Trace.WriteLine(gameModel.Player.Gun.CurrentAmmo);
                }
                else
                {
                    // Reload needed, gun is empty
                    if (gameModel.Player.Gun.EmptySound.Status == SoundStatus.Stopped)
                    {
                        gameModel.Player.Gun.EmptySound.Play();
                    }
                }
            }

            ShakeCameraByRecoil();
        }

        public void PushbackByRecoil(float pushbackValue)
        {
            gameModel.Player.Position -= gameModel.Player.AimDirectionNormalized * pushbackValue;
        }

        public void ShakeCameraByRecoil()
        {
            // Shake camera
            if (gameModel.Player.Gun.CurrentAmmo > 0)
            {
                gameModel.CameraView.Center = new Vector2f(gameModel.CameraView.Center.X + (float)new Random().NextDouble() * gameModel.Player.Gun.Recoil - 2f, gameModel.CameraView.Center.Y + (float)new Random().NextDouble() * gameModel.Player.Gun.Recoil - 2f);
            }
        }

        private void CreateTemporaryShotgunBullet()
        {
            List<BulletModel> shotgunBullets = new List<BulletModel>();

            for (int i = 0; i < 3; i++)
            {
                BulletModel tempShotgunBullet = new BulletModel();
                tempShotgunBullet.Bullet = new Sprite();
                tempShotgunBullet.Speed = 15f;
                tempShotgunBullet.Bullet.Position = gameModel.Player.Gun.Position;
                tempShotgunBullet.Velocity = gameModel.Player.AimDirectionNormalized * tempShotgunBullet.Speed;
                tempShotgunBullet.Bullet.Origin = new Vector2f(tempShotgunBullet.Bullet.TextureRect.Width / 2, tempShotgunBullet.Bullet.TextureRect.Height / 2);
                tempShotgunBullet.Bullet.Scale = new Vector2f(0.5f, 0.5f);

                tempShotgunBullet.Animations = new Dictionary<GunType, AnimationModel>();
                tempShotgunBullet.Animations.Add(GunType.Shotgun, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 10f,
                });

                shotgunBullets.Add(tempShotgunBullet);
            }

            shotgunBullets[0].Bullet.Position = new Vector2f(gameModel.Player.Gun.Position.X - 30f, gameModel.Player.Gun.Position.Y - 15f);
            shotgunBullets[1].Bullet.Position = new Vector2f(gameModel.Player.Gun.Position.X + 30f, gameModel.Player.Gun.Position.Y + 15f);
            shotgunBullets[2].Bullet.Position = new Vector2f(gameModel.Player.Gun.Position.X, gameModel.Player.Gun.Position.Y);

            gameModel.Player.Gun.Bullets.Add(shotgunBullets[0]);
            gameModel.Player.Gun.Bullets.Add(shotgunBullets[1]);
            gameModel.Player.Gun.Bullets.Add(shotgunBullets[2]);
        }

        private void CreateTemporaryRifleBullet()
        {
            BulletModel tempBullet = new BulletModel();
            tempBullet.Bullet = new Sprite();
            tempBullet.Speed = 15f;
            tempBullet.Bullet.Position = gameModel.Player.Gun.Position;
            tempBullet.Velocity = gameModel.Player.AimDirectionNormalized * tempBullet.Speed;
            tempBullet.Bullet.Origin = new Vector2f(tempBullet.Bullet.TextureRect.Width / 2, tempBullet.Bullet.TextureRect.Height / 2);
            tempBullet.Bullet.Scale = new Vector2f(0.5f, 0.5f);

            tempBullet.Animations = new Dictionary<GunType, AnimationModel>();
            tempBullet.Animations.Add(GunType.Rifle, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 8,
                TotalRows = 1,
                TotalColumns = 8,
                Speed = 10f,
            });

            gameModel.Player.Gun.Bullets.Add(tempBullet);
        }

        public void HandleEnemyBulletCollision()
        {
            // Check if player is hit by enemy bullet
            foreach (var enemy in gameModel.Enemies)
            {
                foreach (var bullet in enemy.Gun.Bullets)
                {
                    if (bullet.Bullet.GetGlobalBounds().Intersects(gameModel.Player.Hitbox.GetGlobalBounds()))
                    {
                        if (gameModel.Player.CurrentHP >= 1)
                        {
                            gameModel.Player.CurrentHP -= enemy.Gun.Damage;
                        }

                        enemy.Gun.Bullets.Remove(bullet);

                        if (gameModel.Player.CurrentHP <= 0)
                        {
                            gameModel.Player.IsDead = true;
                            gameModel.Player.DeathCounter++;
                        }
                        return;
                    }
                }
            }
        }

        public void HandleGateCollision()
        {
            for (int i = 0; i < gameModel.Gates.Count; i++)
            {
                if (gameModel.Gates[i].IsGateReady)
                {
                    if (gameModel.Player.GetGlobalBounds().Intersects(gameModel.Gates[i].Hitbox.GetGlobalBounds()))
                    {
                        gameModel.Player.Position = previousPosition;
                    }

                    if (gameModel.Player.GetGlobalBounds().Intersects(gameModel.Gates[i].InteractArea.GetGlobalBounds()))
                    {
                        if (gameModel.Gates[i].GateState == GateState.InKillArena)
                        {
                            if (gameModel.TeleportSound.Status == SoundStatus.Stopped)
                            {
                                gameModel.TeleportSound.Volume = 30;
                                gameModel.TeleportSound.Play();
                            }

                            gameModel.Player.PlayerState = GateState.InKillArena;

                            gameModel.CurrentMap.Vertices = gameModel.KillArenaMap.Vertices;
                            gameModel.CurrentMap.MapLayers = gameModel.KillArenaMap.MapLayers;
                            gameModel.CurrentMap.Width = gameModel.KillArenaMap.Width;
                            gameModel.CurrentMap.Height = gameModel.KillArenaMap.Height;
                            gameModel.CurrentMap.TileWidth = gameModel.KillArenaMap.TileWidth;
                            gameModel.CurrentMap.TileHeight = gameModel.KillArenaMap.TileHeight;
                            gameModel.CurrentMap.Size = new Vector2u(gameModel.KillArenaMap.Width, gameModel.KillArenaMap.Height);
                            gameModel.CurrentMap.TileSize = new Vector2u(gameModel.KillArenaMap.TileWidth, gameModel.KillArenaMap.TileHeight);
                            gameModel.CurrentMap.GateState = Model.Game.Enums.GateState.InKillArena;
                            gameModel.Player.Position = new Vector2f(300f, 150f);

                            for (int j = 0; j < gameModel.Gates.Count; j++)
                            {
                                if (gameModel.Gates[j].GateState == GateState.InLobby)
                                {
                                    gameModel.Gates[j].IsGateReady = true;
                                }
                                else
                                {
                                    gameModel.Gates[j].IsGateReady = false;
                                }
                            }

                            for (int j = 0; j < gameModel.Enemies.Count; j++)
                            {
                                if (gameModel.Enemies[j].EnemyType == EnemyType.Eye)
                                {
                                    gameModel.Enemies[j].CanSpawn = true;
                                }
                                else
                                {
                                    gameModel.Enemies[j].CanSpawn = false;
                                }
                            }
                        }

                        if (gameModel.Player.CurrentXP >= 50)
                        { 
                            if (gameModel.Gates[i].GateState == GateState.InBossArena)
                            {
                                if (gameModel.TeleportSound.Status == SoundStatus.Stopped)
                                {
                                    gameModel.TeleportSound.Volume = 30;
                                    gameModel.TeleportSound.Play();
                                }

                                gameModel.Player.PlayerState = GateState.InBossArena;

                                gameModel.CurrentMap.Vertices = gameModel.BossMap.Vertices;
                                gameModel.CurrentMap.MapLayers = gameModel.BossMap.MapLayers;
                                gameModel.CurrentMap.Width = gameModel.BossMap.Width;
                                gameModel.CurrentMap.Height = gameModel.BossMap.Height;
                                gameModel.CurrentMap.TileWidth = gameModel.BossMap.TileWidth;
                                gameModel.CurrentMap.TileHeight = gameModel.BossMap.TileHeight;
                                gameModel.CurrentMap.Size = new Vector2u(gameModel.BossMap.Width, gameModel.BossMap.Height);
                                gameModel.CurrentMap.TileSize = new Vector2u(gameModel.BossMap.TileWidth, gameModel.BossMap.TileHeight);
                                gameModel.CurrentMap.GateState = Model.Game.Enums.GateState.InBossArena;
                                gameModel.Player.Position = new Vector2f(300f, 500f);

                                for (int j = 0; j < gameModel.Gates.Count; j++)
                                {
                                    gameModel.Gates[j].IsGateReady = false;
                                }

                                // Remove every enemy except boss type
                                //gameModel.Enemies.RemoveAll(x => x.EnemyType != EnemyType.Boss);

                                for (int j = 0; j < gameModel.Enemies.Count; j++)
                                {
                                    if (gameModel.Enemies[j].EnemyType == EnemyType.Boss)
                                    {
                                        gameModel.Enemies[j].CanSpawn = true;
                                    }
                                    else
                                    {
                                        gameModel.Enemies[j].CanSpawn = false;
                                    }
                                }
                            }
                        }


                        if (gameModel.Gates[i].GateState == GateState.InShop)
                        {
                            if (gameModel.TeleportSound.Status == SoundStatus.Stopped)
                            { 
                                gameModel.TeleportSound.Volume = 30;
                                gameModel.TeleportSound.Play();
                            }

                            gameModel.Player.PlayerState = GateState.InShop;
                        }

                        if (gameModel.Gates[i].GateState == GateState.InLobby)
                        {
                            if (gameModel.TeleportSound.Status == SoundStatus.Stopped)
                            {
                                gameModel.TeleportSound.Volume = 30;
                                gameModel.TeleportSound.Play();
                            }

                            gameModel.Player.PlayerState = GateState.InLobby;

                            gameModel.CurrentMap.Vertices = gameModel.LobbyMap.Vertices;
                            gameModel.CurrentMap.MapLayers = gameModel.LobbyMap.MapLayers;
                            gameModel.CurrentMap.Width = gameModel.LobbyMap.Width;
                            gameModel.CurrentMap.Height = gameModel.LobbyMap.Height;
                            gameModel.CurrentMap.TileWidth = gameModel.LobbyMap.TileWidth;
                            gameModel.CurrentMap.TileHeight = gameModel.LobbyMap.TileHeight;
                            gameModel.CurrentMap.Size = new Vector2u(gameModel.LobbyMap.Width, gameModel.LobbyMap.Height);
                            gameModel.CurrentMap.TileSize = new Vector2u(gameModel.LobbyMap.TileWidth, gameModel.LobbyMap.TileHeight);
                            gameModel.CurrentMap.GateState = Model.Game.Enums.GateState.InLobby;
                            gameModel.Player.Position = new Vector2f(300f, 300f);

                            for (int j = 0; j < gameModel.Gates.Count; j++)
                            {
                                if (gameModel.Gates[j].GateState != GateState.InLobby)
                                {
                                    gameModel.Gates[j].IsGateReady = true;
                                }
                                else
                                {
                                    gameModel.Gates[j].IsGateReady = false;
                                }
                            }

                            // Set player's HP to max
                            gameModel.Player.CurrentHP = gameModel.Player.MaxHP;
                        }
                    }
                }
            }
        }

        public void BuyItemFromShop(ICollectibleItem item)
        {
            if (gameModel.Player.Inventory.Capacity < gameModel.Player.Inventory.MaxCapacity && gameModel.Player.CurrentCoins >= item.Price)
            {
                AddItemToInventory(item);

                if (item.ItemType == ItemType.Speed_Potion)
                {
                    gameModel.Player.CurrentCoins -= item.Price;
                }
                else if (item.ItemType == ItemType.Health_Potion)
                {
                    gameModel.Player.CurrentCoins -= item.Price;
                }
            }
        }
    }
}
