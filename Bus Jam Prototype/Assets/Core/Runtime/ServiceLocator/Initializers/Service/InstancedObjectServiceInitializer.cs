namespace Core.ServiceLocator
{
    public class InstancedObjectServiceInitializer<TService> : ServiceInitializer<InstancedObjectServiceInitializer<TService>, TService>
    {
        internal InstancedObjectServiceInitializer() { }
        
        internal TService instance;

        internal override object CreateInstance()
        {
            return instance;
        }
    }
}