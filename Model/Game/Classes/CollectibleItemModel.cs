﻿using Model.Game.Enums;
using Model.Game.Interfaces;
using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Model.Game.Classes
{
    public class CollectibleItemModel : ICollectibleItem
    {
        public int Id { get; set; }
        public Sprite Item { get; set; }
        public bool IsCollected { get; set; }
        public ItemType ItemType { get; set; } = ItemType.Item;
        public Dictionary<ItemType, AnimationModel> Animations { get; set; }
        public bool Spawned { get; set; }
        public SoundBuffer CoinSoundBuffer { get; set; }
        public Sound CoinSound { get; set; }
    }
}
