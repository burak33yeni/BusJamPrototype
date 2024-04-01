using System;

namespace Core.ServiceLocator
{
    internal class SubcontextMisplacementException : Exception
    {
        internal SubcontextMisplacementException(string subcontextName) : base(subcontextName + "subcontext of the project context must be placed at root or as a child of project context.")
        {
        }
    }
}