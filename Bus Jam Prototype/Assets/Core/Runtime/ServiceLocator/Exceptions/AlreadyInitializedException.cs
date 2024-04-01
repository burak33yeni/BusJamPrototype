using System;

namespace Core.ServiceLocator
{
    internal class AlreadyInitializedException : Exception
    {
        internal AlreadyInitializedException() : base("Cannot register service after context initialization.")
        {
        }
    }
}