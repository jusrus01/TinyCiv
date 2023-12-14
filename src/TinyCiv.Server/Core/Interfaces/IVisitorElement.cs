namespace TinyCiv.Server.Core.Interfaces;

public interface IVisitorElement
{
    public void Accept(IVisitor visitor);
}
