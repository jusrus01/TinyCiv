using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Example;

public static class Extensions
{
    public static void Print(this Map map)
    {
        for (int y = 0; y < Constants.Game.HeightSquareCount; y++)
        {
            for (int x = 0; x < Constants.Game.WidthSquareCount; x++)
            {
                if (map.Objects!.Where(o => o.Position!.X == x && o.Position.Y == y).First().Type == GameObjectType.Warrior)
                {
                    Console.Write("W");
                }
                else if (map.Objects!.Where(o => o.Position!.X == x && o.Position.Y == y).First().Type == GameObjectType.City)
                {
                    Console.Write("X");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
    }
}
