using System;
using System.Collections.Generic;
using System.Linq;
using Core.Factory;
using Core.Pool;
using Object = UnityEngine.Object;

namespace Core.ServiceLocator
{
    internal class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly HashSet<object> _serviceInstances = new();
        private Dictionary<Type, ServiceInitializer> _serviceInitializers = new();

        private Context _ownerContext;

        private bool _isInitialized;

        internal ServiceLocator(Context ownerContext)
        {
            _ownerContext = ownerContext;
        }

        internal void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            Type[] typesArr = _serviceInitializers.Keys.ToArray();
            for (int i = 0; i < typesArr.Length; i++)
            {
                Type type = typesArr[i];
                if (!_serviceInitializers.ContainsKey(type)) continue;
                ServiceInitializer initializer = _serviceInitializers[type];
                if (initializer.IsLazy()) continue;
                if (initializer.IsInitialized())
                {
                    _serviceInitializers.Remove(type);
                    continue;
                }

                InitializeService(type, initializer);
            }
        }

        private void InitializeService(Type type, ServiceInitializer initializer)
        {
            object serviceObj = initializer.Initialize();
            Type serviceType = serviceObj.GetType();

            if (initializer.registerTargets.Count == 0)
            {
                AddService(type, serviceObj);
            }
            else
            {
                foreach (Type target in initializer.registerTargets)
                {
                    AddService(target, serviceObj);
                }

                if (!initializer.registerTargets.Contains(serviceType))
                {
                    _serviceInitializers.Remove(serviceType);
                }
            }

            _serviceInstances.Add(serviceObj);
            _ownerContext.FulfillDependencies(serviceObj);
        }

        private void AddService(Type type, object serviceObj)
        {
            _services.Add(type, serviceObj);
            _serviceInitializers.Remove(type);
        }

        internal IncompleteServiceInitializer<TService> Register<TService>()
        {
            if (_isInitialized)
                throw new AlreadyInitializedException();
            Type type = typeof(TService);
            if (!CanRegister(type))
                throw new AlreadyRegisteredException(type);

            IncompleteServiceInitializer<TService> initializer = new()
            {
                initializersRef = _serviceInitializers,
                registerTargets = new HashSet<Type>()
            };

            _serviceInitializers.Add(type, initializer);
            return initializer;
        }

        internal ObjectPoolInitializer<TItem, TPool> RegisterObjectPool<TItem, TPool>(TItem prefab)
            where TPool : BaseObjectPool<TItem>, new() where TItem : Object
        {
            if (_isInitialized)
                throw new AlreadyInitializedException();
            Type type = typeof(TPool);
            if (!CanRegister(type))
                throw new AlreadyRegisteredException(type);

            ObjectPoolInitializer<TItem, TPool> initializer = new()
            {
                initializersRef = _serviceInitializers,
                registerTargets = new HashSet<Type>(),
                prefabObject = prefab,
                parentTransform = _ownerContext.transform,
                ownerContext = _ownerContext
            };

            _serviceInitializers.Add(type, initializer);
            return initializer;
        }

        internal ClassFactoryInitializer<TItem, TFactory> RegisterClassFactory<TItem, TFactory>()
            where TFactory : ClassFactory<TItem>, new() where TItem : class
        {
            if (_isInitialized)
                throw new Exception("Cannot register service after context initialization");
            Type type = typeof(TFactory);
            if (!CanRegister(type))
                throw new Exception("Service already registered");

            ClassFactoryInitializer<TItem, TFactory> initializer = new()
            {
                initializersRef = _serviceInitializers,
                registerTargets = new HashSet<Type>(),
                ownerContext = _ownerContext
            };

            _serviceInitializers.Add(type, initializer);
            return initializer;
        }

        internal ObjectFactoryInitializer<TItem, TFactory> RegisterObjectFactory<TItem, TFactory>(TItem prefab)
            where TFactory : BaseObjectFactory<TItem>, new() where TItem : Object
        {
            if (_isInitialized)
                throw new Exception("Cannot register service after context initialization");
            Type type = typeof(TFactory);
            if (!CanRegister(type))
                throw new Exception("Service already registered");

            ObjectFactoryInitializer<TItem, TFactory> initializer = new()
            {
                initializersRef = _serviceInitializers,
                registerTargets = new HashSet<Type>(),
                ownerContext = _ownerContext,
                prefabObject = prefab,
            };

            _serviceInitializers.Add(type, initializer);
            return initializer;
        }

        internal bool Resolve<TService>(out TService service)
        {
            object serviceObj = GetService(typeof(TService));
            if (serviceObj == null)
            {
                service = default;
                return false;
            }

            service = (TService) serviceObj;
            return true;
        }

        internal bool Resolve(Type type, out object service)
        {
            service = GetService(type);
            if (service == null) return false;
            return true;
        }

        private object GetService(Type type)
        {
            if (IsServiceAvailable(type)) return _services[type];

            if (!IsLazyServiceAvailable(type)) return null;

            InitializeService(type, _serviceInitializers[type]);
            return _services[type];
        }

        internal void ResetServices()
        {
            foreach (KeyValuePair<Type,object> pair in _services)
                if (pair.Value is IDismissible dismissible)
                    dismissible.Dismiss();
            _serviceInitializers.Clear();
            _services.Clear();
            _serviceInstances.Clear();
        }

        private bool CanRegister(Type type)
        {
            if (type.IsSubclassOf(typeof(Context)))
            {
                throw new WrongRegistrationMethodException();
            }

            if (!_services.ContainsKey(type)) return true;
            return false;
        }

        private bool IsLazyServiceAvailable(Type type)
        {
            return _serviceInitializers.ContainsKey(type);
        }

        private bool IsServiceAvailable(Type type)
        {
            return _services.ContainsKey(type);
        }

        internal bool ContainsServiceInstance(object instance)
        {
            return _serviceInstances.Contains(instance);
        }
    }
}