using Core.ServiceLocator;

namespace Core.Factory
{
    public abstract class FactoryModel
    {
        internal FactoryModel() { }
        
        public Context OwnerContext;
    }
}