using System;

namespace Core.Runtime.ServiceLocator.Exceptions
{
    internal class InterfaceNotImplementedByObjectException : Exception
    {
        internal InterfaceNotImplementedByObjectException(Type objInterface, Type obj) :
            base(objInterface + " is not implemented by " + obj + " class.")
        {
        }
    }
}