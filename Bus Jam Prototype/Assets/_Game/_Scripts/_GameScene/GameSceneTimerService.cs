using System;
using System.Collections;
using _Game._Scripts._GameUI;
using _Game._Scripts._GeneralGame;
using Core.ServiceLocator;

namespace _Game._Scripts._GameScene
{
    public class GameSceneTimerService
    {
        [Resolve] private MonoActionService _monoActionService;
        [Resolve] private GameSceneUIService _endGameSceneUIService;
        [Resolve] private CoroutineService _coroutineService;
        
        public void StartConstraintTimer()
        {
            TimerService.StartFreshConstraintTimer();
        }
        public TimeSpan Add30sec()
        {
            return AddRemoveSecToTimer(30f);
        }

        private TimeSpan AddRemoveSecToTimer(float sec)
        {
            TimerService.AddRemoveTimeToConstraintTimer(sec);
            return TimerService.GetRemainingConstraintTime();
        }

        public TimeSpan Remove30sec()
        {
            return AddRemoveSecToTimer(-30f);
        }
    }
}