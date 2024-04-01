using System;

namespace Core.ServiceLocator
{
    internal class ServiceInstanceNotFoundException : Exception
    {
        internal ServiceInstanceNotFoundException(Type type) : base("Service instance of " + type + " can not be found in any of the active contexts.")
        {
        }
    }
}