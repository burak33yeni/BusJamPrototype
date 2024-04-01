using System;

namespace Core.ServiceLocator
{
    internal class ObjectNotPrefabException : Exception
    {
        internal ObjectNotPrefabException() : base("SceneContextCreator can only be used with prefabs")
        {
        }
    }
}