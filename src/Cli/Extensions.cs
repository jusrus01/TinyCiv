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
                var tile = map.Objects!
                    .Where(o => o.Position!.X == x && o.Position.Y == y)
                    .First();

                if (tile.Type == GameObjectType.Empty)
                {
                    Console.Write(".");
                    continue;
                }

                string? enumName = Enum.GetName(tile.Type);

                if (enumName == null)
                {
                    Console.Write("?");
                    continue;
                }

                Console.Write(enumName.ToUpper()[0]);
            }

            Console.WriteLine();
        }
    }
}