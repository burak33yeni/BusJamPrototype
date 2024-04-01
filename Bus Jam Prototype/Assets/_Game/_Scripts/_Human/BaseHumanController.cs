using System;
using System.Collections.Generic;
using _Game._Scripts._BoardArea;
using _Game._Scripts._Buses;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GameScene;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._Human._Events;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Human
{
    public abstract class BaseHumanController
    {
        [Resolve] private HumanControllerHelper _helper;
        [Resolve] private Level _level;
        [Resolve] private HumanEventHelper _eventHelper;
        [Resolve] private GameConstantsAndPositioningService _gameConstantsAndPositioningService;
        [Resolve] private BusStopService _busStopService;
        [Resolve] private BusService _busService;
        private IHumanView _view;
        private Vector2Int _index;
        private HumanBusType _currentBusType;
        private HumanPosition _currentPosition;
        private bool _canMoveNow;
        private Stack<Vector2Int> _validIndexes;
        
        public void Initialize(HumanCreationModel model)
        {
            _view = model.view;
            _index = model.starterIndex;
            _currentPosition = model.currentPosition;
            _currentBusType = model.busType;
            
            _view.Initialize(new HumanViewModel(model.color, model.position, OnClick));
            
            _eventHelper.FollowEvent<BoardIndexEmptyEvent>(OnBoardIndexEmptyEvent);
        }

        private void OnBoardIndexEmptyEvent(BoardIndexEmptyEvent obj)
        {
            if (_currentPosition == HumanPosition.BusStopCell) return;
            SetCanMoveNow();
        }

        public void SetCanMoveNow()
        {
            _canMoveNow = _helper.CanMoveToBusStopCells(_index, out Stack<Vector2Int> validIndexes);
            if (_canMoveNow)
            {
                _validIndexes = validIndexes;
            }
            _view.SetMovementIndicator(_canMoveNow);
        }
        
        private void OnClick()
        {
            if (_currentPosition == HumanPosition.BusStopCell) return;
            if (!_canMoveNow) return;

            if (CheckTryGoingBus(true)) return;
            CheckTryGoingBusStop();
            
        }

        private void CheckTryGoingBusStop()
        {
            if (!CanGoBusStop(out var availableIndex)) return;
            TryGoBusStop(availableIndex);
        }

        private bool CheckTryGoingBus(bool removeFromLevelList)
        {
            if (!CanGoBus(out Vector3 position)) return false;
            if (!TryGoBus(position, removeFromLevelList)) return false;
            _currentPosition = HumanPosition.InBus;
            return true;
        }
        
        public bool IsMoving()
        {
            return _currentPosition == HumanPosition.Moving;
        }
        public bool AutoTriggerToGoBusInBusStop()
        {
            if (_currentPosition != HumanPosition.BusStopCell) return false;
            _validIndexes = new Stack<Vector2Int>();
            if (CheckTryGoingBus(false)) return true;
            return false;
        }

        private void TryGoBusStop(int availableIndex)
        {
            if (!_busStopService.TryAddHumanToCell(this, availableIndex)) return;
            if (!_gameConstantsAndPositioningService.TryGetHumanPathToBusStopCell(
                    _validIndexes, availableIndex, out Stack<Vector3> positions)) return;
            GoToPosition(positions, SendHumanArrivedBusStopEvent, true);
        }

        private void SendHumanArrivedBusStopEvent()
        {
            _eventHelper.SendEvent(new HumanArrivedBusStopEvent());   
        }

        private bool CanGoBusStop(out int availableIndex)
        {
            return _busStopService.TryGetAvailableBusStopCell(out availableIndex);
        }

        private bool TryGoBus(Vector3 passengerPosition, bool removeFromBoardList)
        {
            if (!_gameConstantsAndPositioningService.TryGetHumanPathDirectlyToBus(
                    _validIndexes, passengerPosition, out Stack<Vector3> positions))
            {
                _busService.RemovePassengerFromActiveBus(_currentBusType);
                return false;
            }
            GoToPosition(positions, () =>
            {
                _busService.SetPassengerParentToBusWhenPassengerArrived(_view);
            }, removeFromBoardList);
            return true;
        }

        private bool CanGoBus(out Vector3 position)
        {
            return _busService.TryAddPassengerToActiveBus(_currentBusType, out position);
        }

        private void GoToPosition(Stack<Vector3> positions, Action onArrived, bool removeFromBoardList)
        {
            _currentPosition = HumanPosition.Moving;
            if (removeFromBoardList)
                _level.RemoveHumanOnBoardCell(_index);
            
            _view.GoToPositions(positions, () =>
            {
                _currentPosition = HumanPosition.BusStopCell;
                onArrived?.Invoke();
            });
            _eventHelper.SendEvent(new BoardIndexEmptyEvent(_index));
        }
        
        public bool InBus()
        {
            return _currentPosition == HumanPosition.InBus;
        }
        
        public HumanBusType GetHumanType()
        {
            return _currentBusType;
        }
        
        public IHumanView GetView()
        {
            return _view;
        }
    }
    
    public class HumanCreationModel
    {
        public IHumanView view;
        public Vector2Int starterIndex;
        public CommonColor commonColor;
        public Color color;
        public Vector3 position;
        public HumanBusType busType;
        public HumanPosition currentPosition;
        
        public HumanCreationModel(IHumanView view, Vector2Int starterIndex
        , CommonColor commonColor, Color color, Vector3 position, 
        HumanBusType busType, HumanPosition currentPosition)
        {
            this.view = view;
            this.starterIndex = starterIndex;
            this.color = color;
            this.commonColor = commonColor;
            this.position = position;
            this.busType = busType;
            this.currentPosition = currentPosition;
        }
    }
}