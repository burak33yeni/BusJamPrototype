using System;
using System.Threading.Tasks;
using _Game._Scripts._BoardArea;
using _Game._Scripts._Buses;
using _Game._Scripts._Buses._Events;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._Popups;
using Core.ServiceLocator;

namespace _Game._Scripts._GameScene
{
    public class GameSuccessAndFailService : IInitializable
    {
        [Resolve] private Level _level;
        [Resolve] private BusStopService _busStopService;
        [Resolve] private LevelInventory _levelInventory;
        [Resolve] private HealthInventory _healthInventory;
        [Resolve] private BusService _busService;
        [Resolve] private GameSceneTimerService _gameSceneTimerService;

        private bool _gameEnded;
        public void Initialize()
        {
            _gameEnded = false;
            this.FollowEvent<CheckGameEndedEvent>(OnCheckGameFailOrSuccessEvent);
            this.FollowEvent<BusLeftBusStopEvent>(OnBusLeftBusStopEvent);
        }

        private void OnBusLeftBusStopEvent(BusLeftBusStopEvent obj)
        {
            CheckGameWonDelayed();
        }
        
        private async void CheckGameWonDelayed()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            CheckGameWon();
        }

        private void OnCheckGameFailOrSuccessEvent(CheckGameEndedEvent obj)
        {
            if (CheckGameFail()) return;
            CheckGameWon();
        }

        private bool CheckGameFail()
        {
            if (_busService.TryGetBusAtPeek(out BaseBusController busController) &&
                busController.IsArrived() &&
                !busController.IsFull &&
                _busStopService.IsBusStopFull() &&
                _level.IsThereAnyHumanOnBoard())
            {
                GameFail();
                return true;
            }

            return false;
        }

        private void GameFail()
        {
            if (_gameEnded) return;
            _gameEnded = true;
            KillAnimationTweens();
            _levelInventory.Add<LastPlayedLevelItem>(-1);
            _healthInventory.Add<HealthItem>(-1);
            _gameSceneTimerService.StartConstraintTimer();
            this.SendEvent(new OpenGameFailPopupEvent());
        }
        
        public void ConstraintTimerFinishedInGameScene()
        {
            _healthInventory.Add<HealthItem>(-_healthInventory.Get<HealthItem>());
            _healthInventory.Add<HealthItem>(GameConstantsAndPositioningService.MaxHealth);

        }

        private void CheckGameWon()
        {
            if (!_busService.TryGetBusAtPeek(out BaseBusController busController1) &&
                !_level.IsThereAnyHumanOnBoard() && _busStopService.GetHumansCountInBusStop() <= 0)
            {
                GameWon();
            }
        }

        private void GameWon()
        {
            if (_gameEnded) return;
            _gameEnded = true;
            KillAnimationTweens();
            _levelInventory.Add<LevelItem>(1);
            SceneController.LoadEnd();
        }

        private void KillAnimationTweens()
        {
            this.SendEvent(new KillAllAnimationTweensEvent());
        }
    }

    public class KillAllAnimationTweensEvent : Event
    {
    }

    public class CheckGameEndedEvent : Event
    {
    }
}