using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GeneralGame
{
    public class GlobalMonoInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineService coroutineService;
        [SerializeField] private MonoActionService monoActionService;
        
        public override void Install(Context context)
        {
            context.Register<CoroutineService>().FromInstance(coroutineService).NonLazy();
            context.Register<MonoActionService>().FromInstance(monoActionService);
        }
    }
}
