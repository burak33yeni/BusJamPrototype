using Core.Factory;

namespace _Game._Scripts._Buses._BusFactory
{
    public class BusFactory : ClassFactory<BaseBusController>
    {
        protected override BaseBusController Create<TType>()
        {
            BaseBusController busController = new TType();
            return busController;
        }

        public BaseBusController Spawn<TType>() where TType : BaseBusController, new()
        {
            return base.Spawn<TType>();
        }
        
    }
}