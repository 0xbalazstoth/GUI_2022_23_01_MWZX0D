using Model.Game.Enums;
using SFML.Graphics;
using System;

namespace Model.Game.Interfaces
{
    public interface ICollectibleItem
    {
        int Id { get; set; }

        ItemType ItemType { get; set; }

        Sprite Item { get; set; }

        bool IsCollected {get;set;}
        int Quantity { get; set; }
    }
}
