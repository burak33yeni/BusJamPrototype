using System;
using System.Collections;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GameScene;
using _Game._Scripts._GameUI;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._Popups._PopupFactory;
using Core.ServiceLocator;
using UnityEngine;
using Event = Core.ServiceLocator.Event;

namespace _Game._Scripts._Popups
{
    public class GameFailPopup : IInitializable
    {
        private static readonly  WaitForSeconds WaitOneSecond = new WaitForSeconds(1f);
        
        [Resolve] private CoroutineService _coroutineService;
        [Resolve] private LevelInventory _levelInventory;
        [Resolve] private HealthInventory _healthInventory;
        [Resolve] private GameSceneTimerService _gameSceneTimerService;
        [Resolve] private GameFailPopupFactory _gameFailPopupFactory;
        [Resolve] private GameSceneUIService _gameSceneUIService;
        [Resolve] private GameSuccessAndFailService _gameSuccessAndFailService;
        
        private IGameFailPopupView _view;
        private TimeSpan _remainingConstraintTime;
        private Coroutine _timerCoroutine;

        public void Initialize()
        {
            this.FollowEvent<OpenGameFailPopupEvent>(OpenPopup);
        }
        
        private void OpenPopup(OpenGameFailPopupEvent obj)
        {
            _view = _gameFailPopupFactory.Spawn(_gameSceneUIService.GetPopupParent()); 
            
            int health = _healthInventory.Get<HealthItem>();
            if (health > 0)
            {
                _view.Initialize(new GameFailPopupModel
                {
                    onGoToMenuButtonClicked = OnGoToMenuButtonClicked,
                    onReloadLevelButtonClicked = OnReloadLevelButtonClicked,
                    onAdd30secButtonClicked = OnAdd30secButtonClicked,
                    onRemove30secButtonClicked = OnRemove30secButtonClicked,
                    levelText = "Level " + _levelInventory.Get<LevelItem>() + " failed!",
                    healthText = "Health: " + health,
                    timeText = ""
                });
                _view.OpenCloseReloadGameButtons(true);
            }
            else
            {
                _view.Initialize(new GameFailPopupModel
                {
                    onGoToMenuButtonClicked = OnGoToMenuButtonClicked,
                    onReloadLevelButtonClicked = OnReloadLevelButtonClicked,
                    onAdd30secButtonClicked = OnAdd30secButtonClicked,
                    onRemove30secButtonClicked = OnRemove30secButtonClicked,
                    levelText = "Level " + _levelInventory.Get<LevelItem>() + " failed!",
                    healthText = "Health: " + health,
                    timeText = ""
                });
                _view.OpenCloseReloadGameButtons(false);
                _remainingConstraintTime = TimerService.GetRemainingConstraintTime();
                if (_timerCoroutine != null)
                    _coroutineService.StopCoroutine(_timerCoroutine);
                _timerCoroutine = _coroutineService.StartCoroutine(TimerCoroutine());
            }
        }

        private void OnReloadLevelButtonClicked()
        {
            SceneController.LoadGame();
        }
        
        private IEnumerator TimerCoroutine()
        {
            while (_remainingConstraintTime >= TimeSpan.Zero)
            {
                _view.UpdateTimeText(_remainingConstraintTime
                    .ToString(GameConstantsAndPositioningService.TimerFormat));
                _remainingConstraintTime -= TimeSpan.FromSeconds(1);
                yield return WaitOneSecond;
            }
            _view.OpenCloseReloadGameButtons(true);
        }
        private void OnGoToMenuButtonClicked()
        {
            SceneController.LoadStart();
        }
        
        private void OnAdd30secButtonClicked()
        {
            _remainingConstraintTime = _gameSceneTimerService.Add30sec();
            _view.UpdateTimeText(_remainingConstraintTime
                .ToString(GameConstantsAndPositioningService.TimerFormat));
        }

        private void OnRemove30secButtonClicked()
        {
            _remainingConstraintTime = _gameSceneTimerService.Remove30sec();
            if (_remainingConstraintTime <= TimeSpan.Zero)
            {
                if (_timerCoroutine != null)
                    _coroutineService.StopCoroutine(_timerCoroutine);
                _gameSuccessAndFailService.ConstraintTimerFinishedInGameScene();
                _view.OpenCloseReloadGameButtons(true);
            }
            _view.UpdateTimeText(_remainingConstraintTime
                .ToString(GameConstantsAndPositioningService.TimerFormat));
            
            _view.SetHealthText("Health: " + _healthInventory.Get<HealthItem>());
        }
    }

    public class OpenGameFailPopupEvent : Event
    {
    }
}