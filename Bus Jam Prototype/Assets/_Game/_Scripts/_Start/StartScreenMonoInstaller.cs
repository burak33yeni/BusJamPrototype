using _Game._Scripts._GeneralGame;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Start
{
    public class StartScreenMonoInstaller : MonoInstaller
    {
        [SerializeField] private CanvasService _canvasService;
        [SerializeField] private StartScreenView startScreenView;
        public override void Install(Context context)
        {
            context.Register<CanvasService>().FromInstance(_canvasService).NonLazy();
            context.Register<StartScreenService>().FromNew().WithArguments(startScreenView).NonLazy();

            context.Register<LevelInventory>().FromNew();
            context.Register<HealthInventory>().FromNew();
        }
    }
}