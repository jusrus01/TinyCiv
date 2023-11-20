using TinyCiv.Server.Core.Services;

namespace TinyCiv.Server.Core.Game.Buildings
{
    public abstract class ConstantSpeedBuilding : RecuringBuilding
    {
        protected async override Task Delay()
        {
            await Task.Delay(IntervalMs);
        }
    }
}
