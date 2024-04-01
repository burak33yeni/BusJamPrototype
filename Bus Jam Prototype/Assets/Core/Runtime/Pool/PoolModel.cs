using Core.ServiceLocator;

namespace Core.Pool
{
    public abstract class PoolModel
    {
        internal PoolModel() { }
        
        public int MinimumCount;
        public Context OwnerContext;
    }
}