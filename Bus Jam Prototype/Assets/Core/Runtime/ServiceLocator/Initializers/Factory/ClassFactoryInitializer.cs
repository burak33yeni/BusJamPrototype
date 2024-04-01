using Core.Factory;

namespace Core.ServiceLocator
{
    public class ClassFactoryInitializer<TItem, TFactory> : ServiceInitializer<ClassFactoryInitializer<TItem, TFactory>, TFactory>
        where TFactory : ClassFactory<TItem>, new() where TItem : class
    {
        internal ClassFactoryInitializer() { }
        
        internal Context ownerContext;

        internal override object CreateInstance()
        {
            TFactory factory = new();
            factory.Build(new ClassFactoryModel()
            {
                OwnerContext = ownerContext
            });
            return factory;
        }
    }
}