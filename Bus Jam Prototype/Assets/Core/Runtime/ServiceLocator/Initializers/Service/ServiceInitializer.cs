using System;
using System.Collections.Generic;

namespace Core.ServiceLocator
{
    public abstract class ServiceInitializer
    {
        internal ServiceInitializer() { }
        
        internal HashSet<Type> registerTargets;
        internal Dictionary<Type, ServiceInitializer> initializersRef;
        internal bool isLazy = true;
    
        private object _instance;

        internal virtual bool IsInitialized()
        {
            return _instance != null;
        }
    
        internal object Initialize()
        {
            _instance = CreateInstance();
            return _instance;
        }

        internal virtual bool IsLazy()
        {
            return isLazy;
        }
    
        internal abstract object CreateInstance();
    }

    public abstract class ServiceInitializer<TInitializer, TService> : ServiceInitializer where TInitializer : ServiceInitializer<TInitializer, TService>
    {
        public TInitializer NonLazy()
        {
            isLazy = false;
            return (TInitializer) this;
        }
    }
}