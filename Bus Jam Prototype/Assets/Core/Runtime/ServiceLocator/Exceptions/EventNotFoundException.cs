using System;

namespace Core.ServiceLocator
{
    internal class EventNotFoundException : Exception
    {
        internal EventNotFoundException(Type type) : base("Event of " + type + "can not be found in any context.")
        {
        }
    }
}