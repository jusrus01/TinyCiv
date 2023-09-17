using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCiv.Client.Code.structures
{
    public class City : Structure
    {
        public override GameObjectType Type => GameObjectType.City;

        public City() { }
    }
}
