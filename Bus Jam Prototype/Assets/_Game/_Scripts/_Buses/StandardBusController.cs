using _Game._Scripts._GameConstants;

namespace _Game._Scripts._Buses
{
    public sealed class StandardBusController : BaseBusController
    {
        protected override int MaxPassengers => GameConstantsAndPositioningService.StandardBusMaxPassengers;
        protected override void RemoveBus()
        {
            RemovePassengers();
            busCreator.RemoveStandardBus(_view);
        }
    }
}