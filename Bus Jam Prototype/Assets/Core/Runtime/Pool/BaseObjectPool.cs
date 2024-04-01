using UnityEngine;

namespace Core.Pool
{
    public abstract class BaseObjectPool<TItem> : Pool<Transform, TItem, ObjectPoolModel<TItem>> where TItem : Object
    {
        internal BaseObjectPool() { }

        private Transform _inactiveParent;
        private TItem _prefab;

        internal override void Build(ObjectPoolModel<TItem> model)
        {
            base.Build(model);
            _prefab = model.PrefabObject;
            _inactiveParent = model.ParentTransform;
            Prespawn(minimumCount, model.ParentTransform);
        }
        
        protected sealed override TItem Create(Transform model)
        {
            return Object.Instantiate(_prefab, model);
        }

        protected sealed override void Recreate(TItem obj, Transform model)
        {
            GameObject go = GetGameObject(obj);
            Transform tform = go.transform;
            tform.SetParent(model);
            bool isActive = GetGameObject(_prefab).activeSelf;
            go.SetActive(isActive);
        }

        protected sealed override void Deactivate(TItem obj)
        {
            GameObject go = GetGameObject(obj);
            go.SetActive(false);
            go.transform.SetParent(_inactiveParent);
        }

        protected sealed override void Dispose(TItem obj)
        {
            Object.Destroy(GetGameObject(obj));
        }

        private protected abstract GameObject GetGameObject(TItem obj);
    }
}