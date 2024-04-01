using System;

namespace Core.ServiceLocator
{
    internal class InitializationNotDefinedException : Exception
    {
        internal InitializationNotDefinedException() : base("Initialization method is not defined.")
        {
        }
    }
}