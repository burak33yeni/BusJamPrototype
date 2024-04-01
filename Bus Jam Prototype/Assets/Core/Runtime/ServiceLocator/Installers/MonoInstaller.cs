using UnityEngine;

namespace Core.ServiceLocator
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract void Install(Context context);
    }
}