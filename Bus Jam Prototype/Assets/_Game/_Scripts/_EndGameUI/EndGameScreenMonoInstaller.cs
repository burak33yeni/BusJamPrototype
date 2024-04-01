using _Game._Scripts._GeneralGame;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._EndGameUI
{
    public class EndGameScreenMonoInstaller : MonoInstaller
    {
        [SerializeField] private EndGameSceneUIView endGameSceneUIView;
        public override void Install(Context context)
        {
            context.Register<EndGameSceneUIService>().FromNew().NonLazy()
                .WithArguments(endGameSceneUIView);

            context.Register<LevelInventory>().FromNew();
            context.Register<HealthInventory>().FromNew();
        }
    }
}