using System;

namespace Core.ServiceLocator
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ResolveAttribute : Attribute
    {
    }
}