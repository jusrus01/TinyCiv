using TinyCiv.Server.Game.Buildings;

namespace TinyCiv.Server.Core.Interfaces;

public interface IVisitor
{
    public void Visit(Bank bank);
    public void Visit(Blacksmith blacksmith);
    public void Visit(Farm farm);
    public void Visit(Mine mine);
    public void Visit(Port port);
    public void Visit(Shop shop);
}
