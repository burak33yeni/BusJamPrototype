using System.Collections.Generic;
using _Game._Scripts._Buses._BusFactory;
using _Game._Scripts._Buses._Events;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GameScene;
using _Game._Scripts._Human;
using _Game._Scripts._Human._HumanFactory;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Buses
{
    public abstract class BaseBusController
    {
        [Resolve] private BusEventHelper _busEventHelper;
        [Resolve] private GameConstantsAndPositioningService _gameConstantsAndPositioningService;
        [Resolve] protected BusCreator busCreator;
        [Resolve] protected BusService busService;
        [Resolve] protected HumanCreator humanCreator;
        protected abstract int MaxPassengers { get; }
        private int _currentPassengers;
        private int _arrivedPassengers;
        
        protected IBaseBusView _view;
        public bool IsFull => _currentPassengers >= MaxPassengers;
        private HumanBusType _busType;
        private bool _isArrived;

        protected List<IHumanView> _passengers;

        public void Initialize(BusControllerCreationModel model)
        {
            _passengers = new List<IHumanView>();
            _view = model.view;
            _currentPassengers = model.currentPassengers;
            _arrivedPassengers = model.currentPassengers;
            _busType = model.BusType;
            _isArrived = model.isArrived;
            _busEventHelper.FollowEvent<KillAllAnimationTweensEvent>(OnKillAllAnimationTweensEvent);
        }

        private void OnKillAllAnimationTweensEvent(KillAllAnimationTweensEvent obj)
        {
            _view.KillAllAnimations();
        }

        public bool TryAddPassenger(out Vector3 position)
        {
            position = Vector3.zero;
            if (IsFull) return false; 
            _view.TryGetPassengerPosition(_currentPassengers, out position);
            _currentPassengers++;
            return true;
        }
        
        public void SetPassengerParentWhenPassengerArrived(IHumanView passenger)
        {
            _view.SetPassengersParent(passenger.GetTransform());
            _passengers.Add(passenger);
            _arrivedPassengers++;
            if (IsFull && _arrivedPassengers >= MaxPassengers)
            {
                busService.RemoveBus();
                _busEventHelper.SendEvent(new CreateBusEvent());
                _view.GoPositionAndRemoveBus(
                    Vector3.right * _gameConstantsAndPositioningService.GetFilledBusMaxXPos(),
                    RemoveBus);
                _busEventHelper.SendEvent(new BusLeftBusStopEvent());
            }
        }
        
        public void RemovePassenger()
        {
            _currentPassengers--;
        }
        
        public void MoveToBusStop()
        {
            _view.MoveToPosition(Vector3.zero, () =>
            {
                _isArrived = true;
                _busEventHelper.SendEvent(new BusArrivedToBusStopEvent());
            });
            
        }
        
        public void MoveToQueuedPosition(Vector3 position)
        {
            _view.MoveToPosition(position, null);
        }
        
        public bool IsArrived()
        {
            return _isArrived;
        }
        
        public HumanBusType GetBusType()
        {
            return _busType;
        }

        protected abstract void RemoveBus();
        
        protected void RemovePassengers()
        {
            foreach (var passenger in _passengers)
            {
                humanCreator.RemoveHumanView(passenger as HumanView);
            }
        }
        
        public List<Vector3> GetPassengersPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            for (var i = 0; i < _currentPassengers; i++)
            {
                if (_view.TryGetPassengerPosition(i, out Vector3 position))
                    positions.Add(position);
            }

            return positions;
        }
    }
    
    public struct BusControllerCreationModel
    {
        public IBaseBusView view;
        public int currentPassengers;
        public HumanBusType BusType;
        public bool isArrived;
    }
}