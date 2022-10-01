using Model.Game.Enums;
using Model.Game.Interfaces;
using SFML.Graphics;
using System;

namespace Model.Game.Classes
{
    public class CollectibleItemModel : ICollectibleItem
    {
        public Guid Id { get; set; }
        public CircleShape Item { get; set; }
        public bool IsCollected { get; set; }
        public ItemType type { get; set; }
    }
}
