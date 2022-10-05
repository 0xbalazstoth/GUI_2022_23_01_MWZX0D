using SFML.Graphics;
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
            gameModel.Player.Animations.Add(MovementDirection.Idle, new AnimationModel() {
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
            #endregion

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
            
            #region Bullet animation setup
            foreach (var bullet in gameModel.Player.Bullets)
            {
                bullet.Animations = new Dictionary<GunType, AnimationModel>();
                bullet.Animations.Add(GunType.Pistol, new AnimationModel()
                {
                    Row = 0,
                    ColumnsInRow = 8,
                    TotalRows = 1,
                    TotalColumns = 8,
                    Speed = 7f,
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

            // Coin animation
            foreach (CollectibleItemModel coinItem in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Coin))
            {
                foreach (var itemAnimation in coinItem.Animations)
                {
                    itemAnimation.Value.Counter += itemAnimation.Value.Speed * dt;

                    if (itemAnimation.Value.Counter >= (float)itemAnimation.Value.ColumnsInRow)
                    {
                        itemAnimation.Value.Counter = 0f;
                    }
                    itemAnimation.Value.TextureRect = new IntRect((int)itemAnimation.Value.Counter * itemAnimation.Value.GetSpriteSize.X, itemAnimation.Value.Row * itemAnimation.Value.GetSpriteSize.Y, itemAnimation.Value.GetSpriteSize.X, itemAnimation.Value.GetSpriteSize.Y);
                    coinItem.Animations[itemAnimation.Key].TextureRect = itemAnimation.Value.TextureRect;
                }
            }

            // Health potion animation
            foreach (CollectibleItemModel healthPotionItem in gameModel.CollectibleItems.Where(x => x.ItemType == Model.Game.Enums.ItemType.Health_Potion))
            {
                foreach (var itemAnimation in healthPotionItem.Animations)
                {
                    itemAnimation.Value.Counter += itemAnimation.Value.Speed * dt;

                    if (itemAnimation.Value.Counter >= (float)itemAnimation.Value.ColumnsInRow)
                    {
                        itemAnimation.Value.Counter = 0f;
                    }
                    itemAnimation.Value.TextureRect = new IntRect((int)itemAnimation.Value.Counter * itemAnimation.Value.GetSpriteSize.X, itemAnimation.Value.Row * itemAnimation.Value.GetSpriteSize.Y, itemAnimation.Value.GetSpriteSize.X, itemAnimation.Value.GetSpriteSize.Y);
                    healthPotionItem.Animations[itemAnimation.Key].TextureRect = itemAnimation.Value.TextureRect;
                }
            }

            // Bullet animation
            foreach (var bullet in gameModel.Player.Bullets)
            {
                foreach (var bulletAnimation in bullet.Animations)
                {
                    bulletAnimation.Value.Counter += bulletAnimation.Value.Speed * dt;

                    if (bulletAnimation.Value.Counter >= (float)bulletAnimation.Value.ColumnsInRow)
                    {
                        bulletAnimation.Value.Counter = 0f;
                    }
                    bulletAnimation.Value.TextureRect = new IntRect((int)bulletAnimation.Value.Counter * bulletAnimation.Value.GetSpriteSize.X, bulletAnimation.Value.Row * bulletAnimation.Value.GetSpriteSize.Y, bulletAnimation.Value.GetSpriteSize.X, bulletAnimation.Value.GetSpriteSize.Y);
                    bullet.Animations[bulletAnimation.Key].TextureRect = bulletAnimation.Value.TextureRect;
                }
            }
        }
    }
}
