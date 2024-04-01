using System.Collections.Generic;
using Core.ServiceLocator;

namespace Core.Pool
{
    public abstract class Collector<TItem, TPoolModel> where TItem : class where TPoolModel : PoolModel
    {
        internal Collector() { }
        internal int minimumCount { get; private set; }
        internal HashSet<TItem> allObjects { get; private set; }
        internal Queue<TItem> inactiveItems { get; private set; }
        
        internal Context ownerContext { get; private set; }

        internal virtual void Build(TPoolModel model)
        {
            minimumCount = model.MinimumCount;
            ownerContext = model.OwnerContext;
            allObjects = new HashSet<TItem>();
            inactiveItems = new Queue<TItem>();
        }

        public void Despawn(TItem obj)
        {
            if (obj == null) return;

            if (!allObjects.Contains(obj))
            {
                Dispose(obj);
            }
            else
            {
                inactiveItems.Enqueue(obj);
                Deactivate(obj);
            }
        }

        protected void ConfigureSpawn(TItem obj)
        {
            allObjects.Add(obj);
            ownerContext.FulfillDependencies(obj);
        }
        
        protected virtual void Deactivate(TItem obj) { }
        protected virtual void Dispose(TItem obj) { }
    }
}