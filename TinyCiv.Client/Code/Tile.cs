using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCiv.Client.Code
{
    public class Tile
    {
        public int row { get; private set; }
        public int collumn { get; private set; }
        
        public Tile(int r, int c)
        {
            this.row = r;
            this.collumn = c;
        }
    }
}
