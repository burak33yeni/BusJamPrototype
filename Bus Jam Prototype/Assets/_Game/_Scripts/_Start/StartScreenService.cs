using System;
using System.Collections;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._SaveLoad;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Start
{
    public class StartScreenService : IInitializable
    {
        [Resolve] private LevelInventory _levelInventory;
        [Resolve] private HealthInventory _healthInventory;
        [Resolve] private CoroutineService _coroutineService;
        private StartScreenView _view;
        private static readonly  WaitForSeconds _waitOneSecond = new WaitForSeconds(1f);
        
        private Coroutine _timerCoroutine;
        private int _health;
        private bool _isLevelAvailable;
        private int _level;
        public StartScreenService(StartScreenView view)
        {
            _view = view;
        }

        public void Initialize()
        {
            _level = _levelInventory.Get<LevelItem>();
            _isLevelAvailable = SaveLoadService.TryLoadLevelFromTxt(_level, out LevelSaveObject freshLevel);
            SetTimers();
            InitView();
        }

        private void InitView()
        {
            _view.Init(new StartSceneModel
            {
                level = _isLevelAvailable ? _level.ToString() : _level + " not made yet",
                goLevelButtonText = GetLevelButtonText(),
                goLevelButtonAction = GoLevel,
                add30secButtonAction = Add30sec,
                remove30secButtonAction = Remove30sec,
                isAdd30secButtonActive = _health <= 0,
                isRemove30secButtonActive = _health <= 0
            });
        }

        private void SetTimers()
        {
            CheckFirstTimeOnly();
            _health = _healthInventory.Get<HealthItem>();
            if (_health > 0)
            {
                if (_timerCoroutine != null)
                    _coroutineService.StopCoroutine(_timerCoroutine);
                _view.SetGoLevelButtonActivenessAsColor(_isLevelAvailable);
                _view.SetTimerText(_health + " Health remaining");
            }
            else if (TimerService.CanPlayGame(out TimeSpan timeSpan))
            {
                _healthInventory.Add<HealthItem>(GameConstantsAndPositioningService.MaxHealth - _health);
                _health = _healthInventory.Get<HealthItem>();
                if (_timerCoroutine != null)
                    _coroutineService.StopCoroutine(_timerCoroutine);
                _view.SetGoLevelButtonActivenessAsColor(_isLevelAvailable);
                _view.SetTimerText("Timer ended! " + _health + " Health remaining");
            }
            else
            {
                _view.SetGoLevelButtonActivenessAsColor(false);
                _timerCoroutine = _coroutineService.StartCoroutine(
                    ConstraintTimerCoroutine(timeSpan));   
            }
        }

        private void CheckFirstTimeOnly()
        {
            if (_healthInventory.Get<FirstTimeCheck>() != 0) return;
            _healthInventory.Add<FirstTimeCheck>(1);
            _healthInventory.Add<HealthItem>(GameConstantsAndPositioningService.MaxHealth);
        }

        private IEnumerator ConstraintTimerCoroutine(TimeSpan remainingConstraintTime)
        {
            while (remainingConstraintTime > TimeSpan.Zero)
            {
                _view.SetTimerText(
                    "Wait " + 
                    remainingConstraintTime.ToString("mm\\:ss") +
                    " to play again");
                remainingConstraintTime = remainingConstraintTime.Subtract(TimeSpan.FromSeconds(1));
                yield return _waitOneSecond;
            }
            _view.SetGoLevelButtonActivenessAsColor(_isLevelAvailable);
            int health = _healthInventory.Get<HealthItem>();
            if (health <= 0)
            {
                _healthInventory.Add<HealthItem>(GameConstantsAndPositioningService.MaxHealth - health);
            }
            SetTimers();
            _view.SetAddRemoveTimeButtonsActiveness(false, false);
        }
        
        private void GoLevel()
        {
            if (!_isLevelAvailable)
                return;
            if (_health > 0)
            {
                SceneController.LoadGame();
            }
        }

        private string GetLevelButtonText()
        {
            if (!_isLevelAvailable)
            {
                return "All levels are completed!";
            }
            if (_levelInventory.Get<LastPlayedLevelItem>() < _levelInventory.Get<LevelItem>())
            {
                return "Start level " + _levelInventory.Get<LevelItem>();
            }
            else
            {
                return "Continue level " + _levelInventory.Get<LevelItem>();
            }
        }
        
        private void Add30sec()
        {
            AddRemoveTime(30f);
        }

        private void AddRemoveTime(float sec)
        {
            if (TimerService.CanPlayGame(out TimeSpan timeSpan))
                return;
            TimerService.AddRemoveTimeToConstraintTimer(sec);
            TimeSpan remainingConstraintTime = TimerService.GetRemainingConstraintTime();
            if (remainingConstraintTime > TimeSpan.Zero)
            {
                _coroutineService.StopCoroutine(_timerCoroutine);
                _timerCoroutine = _coroutineService.StartCoroutine(
                    ConstraintTimerCoroutine(remainingConstraintTime));
            }
            else
            {
                SetTimers();
            }
        }

        private void Remove30sec()
        {
            AddRemoveTime(-30f);
        }
    }
}