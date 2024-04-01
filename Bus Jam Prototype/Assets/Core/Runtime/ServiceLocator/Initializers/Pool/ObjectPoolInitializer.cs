using Core.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.ServiceLocator
{
    public class ObjectPoolInitializer<TItem, TPool> : ServiceInitializer<ObjectPoolInitializer<TItem, TPool>, TPool>
        where TItem : Object where TPool : BaseObjectPool<TItem>, new()
    {
        internal ObjectPoolInitializer() { }
        
        internal TItem prefabObject;
        internal Transform parentTransform;
        internal Context ownerContext;

        private int _minimumCount;

        internal override object CreateInstance()
        {
            TPool objectPool = new();
            objectPool.Build(new ObjectPoolModel<TItem>()
            {
                PrefabObject = prefabObject,
                ParentTransform = parentTransform,
                MinimumCount = _minimumCount,
                OwnerContext = ownerContext
            });
            return objectPool;
        }

        public ObjectPoolInitializer<TItem, TPool> WithParent(Transform parent)
        {
            parentTransform = parent;
            return this;
        }

        public ObjectPoolInitializer<TItem, TPool> WithMinimumCount(int count)
        {
            _minimumCount = count;
            return this;
        }
    }
}