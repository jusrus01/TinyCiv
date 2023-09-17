using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class Position
    {
        public int row;
        public int column;

        public Position(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public Position(ServerPosition serverPosition)
        {
            row = serverPosition.row;
            column = serverPosition.column;
        }

        public override bool Equals(object obj)
        {
            if(obj is Position)
            {
                var other = (Position)obj;
                return other.row == this.row && other.column == this.column;
            }
            return false;
        }

        public Position Direction()
        {
            return new Position(Math.Sign(this.row), Math.Sign(this.column));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(row, column);
        }

        public static bool operator == (Position a, Position b)
        {
            return a.Equals(b);
        }

        public static bool operator != (Position a, Position b)
        {
            return !a.Equals(b);
        }

        public static Position operator + (Position a, Position b)
        {
            return new Position(a.row + b.row, a.column + b.column);
        }

        public static Position operator - (Position a, Position b)
        {
            return new Position(a.row - b.row, a.column - b.column);
        }


    }
}
