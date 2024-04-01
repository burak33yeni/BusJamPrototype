using UnityEngine;

namespace Core.Factory
{
    public abstract class ObjectFactory : BaseObjectFactory<GameObject>
    {
    }
    
    public abstract class ObjectFactory<TComponent> : BaseObjectFactory<TComponent> where TComponent : Component
    {
    }
}