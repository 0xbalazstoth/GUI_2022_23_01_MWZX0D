using Model.Game.Enums;
using SFML.Graphics;
using System;

namespace Model.Game.Interfaces
{
    public interface ICollectibleItem
    {
        Guid Id { get; set; }

        ItemType type {get;set;}

        Sprite Item { get; set; }

        bool IsCollected {get;set;}
    }
}
