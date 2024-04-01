using System;

namespace Core.ServiceLocator
{
    internal class ServiceNotFoundException : Exception
    {
        internal ServiceNotFoundException(Type type) : base("Service of " + type + " can not be found in any context.")
        {
        }
    }
}