using System;

namespace Core.ServiceLocator
{
    public class ConstructorObjectServiceInitializer<TService> : ServiceInitializer<ConstructorObjectServiceInitializer<TService>, TService>
    {
        internal ConstructorObjectServiceInitializer() { }
        
        private object[] _args;

        internal override object CreateInstance()
        {
            return Activator.CreateInstance(typeof(TService), _args);
        }

        public ConstructorObjectServiceInitializer<TService> WithArguments(params object[] arguments)
        {
            _args = arguments;
            return this;
        }
    }
}