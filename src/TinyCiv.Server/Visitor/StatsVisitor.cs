using System.Text.Json;
using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Server.Game.Buildings;

namespace TinyCiv.Server.Visitor;

public class StatsVisitor : IVisitor
{
    public void Visit(Bank bank)
    {
        var data = new
        {
            bank.BuildingType,
            bank.TileType,
            bank.Price,
            bank.IntervalMs
        };

        Console.WriteLine(JsonSerializer.Serialize(data));
    }

    public void Visit(Blacksmith blacksmith)
    {
        var data = new
        {
            blacksmith.BuildingType,
            blacksmith.TileType,
            blacksmith.Price,
            blacksmith.IntervalMs
        };

        Console.WriteLine(JsonSerializer.Serialize(data));
    }

    public void Visit(Farm farm)
    {
        var data = new
        {
            farm.BuildingType,
            farm.TileType,
            farm.Price,
            farm.IntervalMs
        };

        Console.WriteLine(JsonSerializer.Serialize(data));
    }

    public void Visit(Mine mine)
    {
        var data = new
        {
            mine.BuildingType,
            mine.TileType,
            mine.Price,
            mine.IntervalMs
        };

        Console.WriteLine(JsonSerializer.Serialize(data));
    }

    public void Visit(Port port)
    {
        var data = new
        {
            port.BuildingType,
            port.TileType,
            port.Price,
            port.IntervalMs
        };

        Console.WriteLine(JsonSerializer.Serialize(data));
    }

    public void Visit(Shop shop)
    {
        var data = new
        {
            shop.BuildingType,
            shop.TileType,
            shop.Price,
            shop.IntervalMs
        };

        Console.WriteLine(JsonSerializer.Serialize(data));
    }
}
