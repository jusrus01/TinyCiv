using System;
using System.Windows.Threading;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units;

public abstract class Unit : GameObject
{
    protected Unit(ServerGameObject serverGameObject) : base(serverGameObject)
    {
    }

}
