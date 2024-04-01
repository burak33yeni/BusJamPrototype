using Core.Factory;
using UnityEngine;

namespace Core.ServiceLocator
{
    public class ObjectFactoryInitializer<TItem, TFactory> : ServiceInitializer<ObjectFactoryInitializer<TItem, TFactory>, TFactory>
        where TItem : Object where TFactory : BaseObjectFactory<TItem>, new()
    {
        internal ObjectFactoryInitializer() { }
        
        internal Context ownerContext;
        internal TItem prefabObject;

        internal override object CreateInstance()
        {
            TFactory objectFactory = new();
            objectFactory.Build(new ObjectFactoryModel<TItem>()
            {
                OwnerContext = ownerContext,
                PrefabObject = prefabObject,
            });
            return objectFactory;
        }
    }
}