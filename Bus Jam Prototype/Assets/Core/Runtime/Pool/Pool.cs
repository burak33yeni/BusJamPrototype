using Core.Runtime.ServiceLocator.Exceptions;

namespace Core.Pool
{
    public abstract class Pool<TModel, TItem, TPoolModel> : Collector<TItem, TPoolModel> where TItem : class where TPoolModel : PoolModel
    {
        internal Pool() { }
        
        public TInterface Spawn<TInterface>(TModel arg) where TInterface : class
        {
            if (inactiveItems.Count > 0)
            {
                TItem obj = inactiveItems.Dequeue();
                Recreate(obj, arg);
                if (obj is not TInterface objInterface)
                    throw new InterfaceNotImplementedByObjectException(typeof(TInterface), obj.GetType());
                return objInterface;
            }
            else
            {
                TItem obj = Create(arg);
                ConfigureSpawn(obj);
                if (obj is not TInterface objInterface)
                    throw new InterfaceNotImplementedByObjectException(typeof(TInterface), obj.GetType());
                return objInterface;
            }
        }

        private protected void Prespawn(int count, TModel model)
        {
            for (int i = 0; i < count; i++)
            {
                TItem obj = Create(model);
                Deactivate(obj);
                inactiveItems.Enqueue(obj);
                allObjects.Add(obj);
                ownerContext.FulfillDependencies(obj);
            }
        }
        
        protected abstract TItem Create(TModel model);
        protected abstract void Recreate(TItem obj, TModel model);
    }
}