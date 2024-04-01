using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Factory
{
    public abstract class BaseObjectFactory<TItem> : Factory<Transform, TItem, ObjectFactoryModel<TItem>> where TItem : Object
    {
        internal BaseObjectFactory() { }
        
        private TItem _prefabObject;
        
        internal override void Build(ObjectFactoryModel<TItem> model)
        {
            _prefabObject = model.PrefabObject;
            base.Build(model);
        }

        protected sealed override TItem Create(Transform parent)
        {
            return Object.Instantiate(_prefabObject, parent, true);
        }
    }
}