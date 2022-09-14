using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Game.Entities
{
    public class Chest : ItemEntity
    {
        public Chest()
        {
            Size = new(32, 32);
        }

        public override void Update(float dt)
        {

        }

        public override string ToString()
        {
            return "[Type]: Item; [Object]: Chest";
        }
    }
}
