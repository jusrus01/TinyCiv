using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Server.Game.Buildings;
using TinyCiv.Shared;

namespace TinyCiv.Server.Visitor;

public class ModsVisitor : IVisitor
{
    public void Visit(Bank bank)
    {
        bank.IntervalMs = new Random().Next(0, Constants.Game.BlacksmithInterval);
    }

    public void Visit(Blacksmith blacksmith)
    {
        blacksmith.IntervalMs = new Random().Next(0, Constants.Game.BankInterval);
    }

    public void Visit(Farm farm)
    {
        farm.IntervalMs = new Random().Next(0, Constants.Game.MineInterval);
    }

    public void Visit(Mine mine)
    {
        mine.IntervalMs = new Random().Next(0, Constants.Game.PortInterval);
    }

    public void Visit(Port port)
    {
        port.IntervalMs = new Random().Next(0, Constants.Game.ShopInterval);
    }

    public void Visit(Shop shop)
    {
        shop.IntervalMs = new Random().Next(0, Constants.Game.FarmInterval);
    }
}
