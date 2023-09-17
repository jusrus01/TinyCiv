using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TinyCiv.Client.Code.units
{
    public class Cavalry : Unit
    {
        public override GameObjectType Type => GameObjectType.Cavalry;

        public Cavalry(int playerId, int r, int c) : base(playerId, r, c)
        {
        }

    }
}
