using System.Collections.Generic;

namespace Core.ServiceLocator
{
    public abstract class ContextInitializer
    {
        internal ContextInitializer() { }
        
        internal HashSet<ContextInitializer> initializersRef;
    
        private Context _instance;
    
        internal Context Initialize()
        {
            _instance = CreateInstance();
            return _instance;
        }

        internal abstract Context CreateInstance();
    }
}