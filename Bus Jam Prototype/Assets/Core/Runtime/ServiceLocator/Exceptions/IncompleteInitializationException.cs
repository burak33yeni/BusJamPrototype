using System;

namespace Core.ServiceLocator
{
    internal class IncompleteInitializationException : Exception
    {
        internal IncompleteInitializationException() : base("The method or operation is not implemented.")
        {
        }
    }
}