using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Game.Classes
{
    public class PlayerModel : UnitEntityModel
    {
        public string Name { get; set; }
        public Vector2f AimDirection { get; set; }
        public Vector2f AimDirectionNormalized { get; set; }
        public Dictionary<MovementDirection, AnimationModel> Animations { get; set; }
        public InventoryModel Inventory { get; set; }
        public bool IsFocusedInGame { get; set; } = true;
        public int CurrentXP { get; set; }
        public int CurrentCoins { get; set; }
    }
}
