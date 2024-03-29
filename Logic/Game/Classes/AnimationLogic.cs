﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Game.Interfaces;
using Model.Game.Classes;
using Model.Game;
using Model.Game.Enums;

namespace Logic.Game.Classes
{
    public class AnimationLogic : IAnimationLogic
    {
        private IGameModel gameModel;

        public AnimationLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;

            #region Player animation setup
            gameModel.Player.Animations = new Dictionary<MovementDirection, AnimationModel>();
            gameModel.Player.Animations.Add(MovementDirection.Idle, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 5,
                TotalRows = 1,
                TotalColumns = 5,
                Speed = 10f,
            });

            gameModel.Player.Animations.Add(MovementDirection.IdleLeft, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 5,
                TotalRows = 1,
                TotalColumns = 5,
                Speed = 10f,
            });

            gameModel.Player.Animations.Add(MovementDirection.IdleRight, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 5,
                TotalRows = 1,
                TotalColumns = 5,
                Speed = 10f,
            });

            gameModel.Player.Animations.Add(MovementDirection.Left, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 4,
                TotalRows = 1,
                TotalColumns = 4,
                Speed = 10f,
            });

            gameModel.Player.Animations.Add(MovementDirection.Right, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 4,
                TotalRows = 1,
                TotalColumns = 4,
                Speed = 10f,
            });

            gameModel.Player.Animations.Add(MovementDirection.Up, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 8,
                TotalRows = 1,
                TotalColumns = 8,
                Speed = 10f,
            });

            gameModel.Player.Animations.Add(MovementDirection.Down, new AnimationModel()
            {
                Row = 0,
                ColumnsInRow = 8,
                TotalRows = 1,
                TotalColumns = 8,
                Speed = 10f,
            });
            #endregion

            #region Item animation setup
            foreach (CollectibleItemModel coin in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Coin))
            {
                coin.Animations = new Dictionary<ItemType, AnimationModel>();
                coin.Animations.Add(ItemType.Coin, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 2,
                    TotalRows = 1,
                    TotalColumns = 2,
                    Speed = 3f,
                });
            }

            foreach (CollectibleItemModel healthPotion in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Health_Potion))
            {
                healthPotion.Animations = new Dictionary<ItemType, AnimationModel>();
                healthPotion.Animations.Add(ItemType.Health_Potion, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 2,
                    TotalRows = 1,
                    TotalColumns = 2,
                    Speed = 3f,
                });
            }

            foreach (CollectibleItemModel speedPotion in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Speed_Potion))
            {
                speedPotion.Animations = new Dictionary<ItemType, AnimationModel>();
                speedPotion.Animations.Add(ItemType.Speed_Potion, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 2,
                    TotalRows = 1,
                    TotalColumns = 2,
                    Speed = 3f,
                });
            }
            #endregion

            #region Enemies animation setup
            foreach (EnemyModel enemy in gameModel.Enemies.Where(x => x.EnemyType == EnemyType.Eye))
            {
                enemy.Animations = new Dictionary<MovementDirection, AnimationModel>();

                enemy.Animations.Add(MovementDirection.Left, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 10f,
                });

                enemy.Animations.Add(MovementDirection.Right, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 10f,
                });
            }

            foreach (EnemyModel enemy in gameModel.Enemies.Where(x => x.EnemyType == EnemyType.Boss))
            {
                enemy.Animations = new Dictionary<MovementDirection, AnimationModel>();

                enemy.Animations.Add(MovementDirection.Left, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 10f,
                });

                enemy.Animations.Add(MovementDirection.Right, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 10f,
                });
            }
            #endregion

            #region Gate animation setup
            foreach (GateModel gate in gameModel.Gates)
            {
                gate.Animations = new List<AnimationModel>();

                gate.Animations.Add(new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 16,
                    TotalRows = 1,
                    TotalColumns = 16,
                    Speed = 10f,
                });
            }
            #endregion
        }

        public void Update(float dt)
        {
            // Player animation
            foreach (var playerAnimation in gameModel.Player.Animations)
            {
                playerAnimation.Value.Counter += playerAnimation.Value.Speed * dt;

                if (playerAnimation.Value.Counter >= (float)playerAnimation.Value.ColumnsInRow)
                {
                    playerAnimation.Value.Counter = 0f;
                }
                playerAnimation.Value.TextureRect = new IntRect((int)playerAnimation.Value.Counter * playerAnimation.Value.GetSpriteSize.X, playerAnimation.Value.Row * playerAnimation.Value.GetSpriteSize.Y, playerAnimation.Value.GetSpriteSize.X, playerAnimation.Value.GetSpriteSize.Y);
                gameModel.Player.Animations[playerAnimation.Key].TextureRect = playerAnimation.Value.TextureRect;
            }

            // Item animation
            if (gameModel.Player.PlayerState == GateState.InKillArena)
            {
                foreach (CollectibleItemModel item in gameModel.CollectibleItems)
                {
                    foreach (var itemAnimation in item.Animations)
                    {
                        itemAnimation.Value.Counter += itemAnimation.Value.Speed * dt;

                        if (itemAnimation.Value.Counter >= (float)itemAnimation.Value.ColumnsInRow)
                        {
                            itemAnimation.Value.Counter = 0f;
                        }
                        itemAnimation.Value.TextureRect = new IntRect((int)itemAnimation.Value.Counter * itemAnimation.Value.GetSpriteSize.X, itemAnimation.Value.Row * itemAnimation.Value.GetSpriteSize.Y, itemAnimation.Value.GetSpriteSize.X, itemAnimation.Value.GetSpriteSize.Y);
                        item.Animations[itemAnimation.Key].TextureRect = itemAnimation.Value.TextureRect;
                    }
                }
            }

            for (int i = 0; i < gameModel.Player.Gun.Bullets.Count; i++)
            {
                foreach (var bulletAnimation in gameModel.Player.Gun.Bullets[i].Animations)
                {
                    bulletAnimation.Value.Counter += bulletAnimation.Value.Speed * dt;

                    if (bulletAnimation.Value.Counter >= (float)bulletAnimation.Value.ColumnsInRow)
                    {
                        bulletAnimation.Value.Counter = 0f;
                    }

                    bulletAnimation.Value.TextureRect = new IntRect((int)bulletAnimation.Value.Counter * bulletAnimation.Value.GetSpriteSize.X, bulletAnimation.Value.Row * bulletAnimation.Value.GetSpriteSize.Y, bulletAnimation.Value.GetSpriteSize.X, bulletAnimation.Value.GetSpriteSize.Y);
                    gameModel.Player.Gun.Bullets[i].Animations[bulletAnimation.Key].TextureRect = bulletAnimation.Value.TextureRect;
                }
            }

            if (gameModel.Player.PlayerState == Model.Game.Enums.GateState.InKillArena || gameModel.Player.PlayerState == Model.Game.Enums.GateState.InBossArena)
            {
                // Enemy bullet animation
                for (int i = 0; i < gameModel.Enemies.Count; i++)
                {
                    if (gameModel.Enemies[i].CanSpawn)
                    {
                        for (int j = 0; j < gameModel.Enemies[i].Gun.Bullets.Count; j++)
                        {
                            foreach (var bulletAnimation in gameModel.Enemies[i].Gun.Bullets[j].Animations)
                            {
                                bulletAnimation.Value.Counter += bulletAnimation.Value.Speed * dt;

                                if (bulletAnimation.Value.Counter >= (float)bulletAnimation.Value.ColumnsInRow)
                                {
                                    bulletAnimation.Value.Counter = 0f;
                                }

                                bulletAnimation.Value.TextureRect = new IntRect((int)bulletAnimation.Value.Counter * bulletAnimation.Value.GetSpriteSize.X, bulletAnimation.Value.Row * bulletAnimation.Value.GetSpriteSize.Y, bulletAnimation.Value.GetSpriteSize.X, bulletAnimation.Value.GetSpriteSize.Y);
                                gameModel.Enemies[i].Gun.Bullets[j].Animations[bulletAnimation.Key].TextureRect = bulletAnimation.Value.TextureRect;
                            }
                        }
                    }
                }

                // Enemy animation
                for (int i = 0; i < gameModel.Enemies.Count; i++)
                {
                    if (gameModel.Enemies[i].CanSpawn)
                    {
                        foreach (var enemyAnimation in gameModel.Enemies[i].Animations)
                        {
                            enemyAnimation.Value.Counter += enemyAnimation.Value.Speed * dt;

                            if (enemyAnimation.Value.Counter >= (float)enemyAnimation.Value.ColumnsInRow)
                            {
                                enemyAnimation.Value.Counter = 0f;
                            }

                            enemyAnimation.Value.TextureRect = new IntRect((int)enemyAnimation.Value.Counter * enemyAnimation.Value.GetSpriteSize.X, enemyAnimation.Value.Row * enemyAnimation.Value.GetSpriteSize.Y, enemyAnimation.Value.GetSpriteSize.X, enemyAnimation.Value.GetSpriteSize.Y);
                            gameModel.Enemies[i].Animations[enemyAnimation.Key].TextureRect = enemyAnimation.Value.TextureRect;
                        }
                    }
                }
            }
            
            // Gate animation
            for (int i = 0; i < gameModel.Gates.Count; i++)
            {
                if (gameModel.Gates[i].IsGateReady)
                {
                    for (int j = 0; j < gameModel.Gates[i].Animations.Count; j++)
                    {
                        gameModel.Gates[i].Animations[j].Counter += gameModel.Gates[i].Animations[j].Speed * dt;

                        if (gameModel.Gates[i].Animations[j].Counter >= (float)gameModel.Gates[i].Animations[j].ColumnsInRow)
                        {
                            gameModel.Gates[i].Animations[j].Counter = 0f;
                        }

                        gameModel.Gates[i].Animations[j].TextureRect = new IntRect((int)gameModel.Gates[i].Animations[j].Counter * gameModel.Gates[i].Animations[j].GetSpriteSize.X, gameModel.Gates[i].Animations[j].Row * gameModel.Gates[i].Animations[j].GetSpriteSize.Y, gameModel.Gates[i].Animations[j].GetSpriteSize.X, gameModel.Gates[i].Animations[j].GetSpriteSize.Y);
                    }
                }
            }
        }
    }
}
