using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Entities
{
    public class Enemy : UnitEntity
    {
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public string Name { get; set; }

        public override void Update(float dt)
        {
            DeltaTime = dt;
            Speed = 150f;
        }

        protected override void HandleMovement()
        {
            // Chase player
            // https://code.markrichards.ninja/sfml/top-down-shoot-em-up-mechanics-part-1
        }
        public override void RedrawTexture(float dt, Texture[] texture, IntRect[] textureRect)
        {

        }

        public override string ToString()
        {
            return "[Type]: Unit; [Object]: Enemy";
        }

        public override void LoadTexture(Texture texture)
        {
            
        }
    }
}
