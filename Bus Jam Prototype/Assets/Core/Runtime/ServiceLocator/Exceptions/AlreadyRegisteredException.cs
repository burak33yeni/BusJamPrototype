using System;

namespace Core.ServiceLocator
{
    internal class AlreadyRegisteredException : Exception
    {
        internal AlreadyRegisteredException(Type type) : base("Service of " + type + " is already registered.")
        {
        }
    }
}