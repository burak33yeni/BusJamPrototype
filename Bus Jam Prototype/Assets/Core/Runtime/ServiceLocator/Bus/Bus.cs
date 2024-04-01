using System;
using System.Collections.Generic;
using Core.Tools;

namespace Core.ServiceLocator
{
    internal abstract class Bus<TBase>
    {
        private Dictionary<Type, Map<long, object>> _typeMapDict;
        
        private const long GAP_FACTOR = long.MaxValue / int.MaxValue / 2;
        
        internal Bus()
        {
            _typeMapDict = new Dictionary<Type, Map<long, object>>();
        }

        internal void Register<T>() where T : TBase
        {
            if (HasType<T>()) 
                throw new Exception("Event already registered");
            _typeMapDict[typeof(T)] = new Map<long, object>();
        }
    
        internal void Follow<T>(Action<T> action, int invocationOrder = 0) where T : TBase
        {
            Follow(typeof(T), action, invocationOrder);
        }
    
        protected void Follow(Type type, object action, int invocationOrder = 0)
        {
            Map<long, object> actionsMap = GetTypeMap(type);

            long longInvocationOrder = invocationOrder * GAP_FACTOR;
            
            while (actionsMap.ContainsKey(longInvocationOrder)) 
                longInvocationOrder++;

            actionsMap.Add(longInvocationOrder, action);
        }
        
        protected internal bool HasType<T>() where T : TBase
        {
            return _typeMapDict.ContainsKey(typeof(T));
        }
        
        protected Map<long, object> GetTypeMap(Type type)
        {
            return _typeMapDict[type];
        }
    }
}