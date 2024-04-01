using Core.ServiceLocator;

public static class InitializerExtensions
{
    public static ConstructorObjectServiceInitializer<TService> FromNew<TService>(this IncompleteServiceInitializer<TService> initializer)
    {
        ConstructorObjectServiceInitializer<TService> newInitializer = new()
        {
            isLazy = initializer.isLazy,
            initializersRef = initializer.initializersRef, 
            registerTargets = initializer.registerTargets
        };
        initializer.initializersRef[typeof(TService)] = newInitializer;
        return newInitializer;
    }

    public static InstancedObjectServiceInitializer<TService> FromInstance<TService>(this IncompleteServiceInitializer<TService> initializer, TService instance)
    {
        InstancedObjectServiceInitializer<TService> newInitializer = new()
        {
            isLazy = initializer.isLazy,
            initializersRef = initializer.initializersRef,
            registerTargets = initializer.registerTargets,
            instance = instance
        };
        initializer.initializersRef[typeof(TService)] = newInitializer;
        return newInitializer;
    }
}