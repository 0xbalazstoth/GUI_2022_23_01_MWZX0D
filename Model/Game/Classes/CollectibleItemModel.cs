using Model.Game.Enums;
using Model.Game.Interfaces;
using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Model.Game.Classes
{
    public class CollectibleItemModel : ICollectibleItem
    {
        public Guid Id { get; set; }
        public Sprite Item { get; set; }
        public bool IsCollected { get; set; }
        public ItemType ItemType { get; set; } = ItemType.Item;
        public Dictionary<ItemType, AnimationModel> Animations { get; set; }
    }
}
