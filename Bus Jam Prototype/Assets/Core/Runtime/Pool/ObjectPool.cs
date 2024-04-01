using UnityEngine;

namespace Core.Pool
{
    public abstract class ObjectPool<TComponent> : BaseObjectPool<TComponent> where TComponent : Component
    {
        private protected sealed override GameObject GetGameObject(TComponent obj)
        {
            return obj.gameObject;
        }
    }
}