using Model.Game.Enums;
using SFML.Graphics;
using System;

namespace Model.Game.Interfaces
{
    public interface ICollectibleItem
    {
        Guid Id { get; set; }

        ItemType ItemType { get; set; }

        CircleShape Item { get; set; }

        bool IsCollected {get;set;}
    }
}
