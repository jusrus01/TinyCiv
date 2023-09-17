using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCiv.Client.Code.units
{
    public class Warrior : Unit
    {
        public override GameObjectType Type => GameObjectType.Warrior;

        public Warrior(int playerId, int r, int c) : base(playerId, r, c)
        {
        }

    }
}
