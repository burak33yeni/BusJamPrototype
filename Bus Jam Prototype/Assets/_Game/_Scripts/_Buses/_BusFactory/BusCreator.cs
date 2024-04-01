using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Game._Scripts._BoardArea;
using _Game._Scripts._Buses._Events;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GameScene;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Buses._BusFactory
{
    public class BusCreator
    {
        private static readonly float _busCreationDelay = 1f;
        [Resolve] private Level _level;
        [Resolve] private BusFactory _busFactory;
        [Resolve] private StandardBusViewPool _standardBusViewPool;
        [Resolve] private BoardAreaService _boardAreaService;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        [Resolve] private GameConstantsAndPositioningService _gameConstantsAndPositioningService;
        [Resolve] private BusService _busService;

        private Stack<BusCreationModel> _busCreationModels;
        
        public void Initialize(Stack<BusCreationModel> _busCreationModels)
        {
            this._busCreationModels = _busCreationModels;
            if (TryCreateNewBus(Vector3.zero, true, out BaseBusController bus))
            {
                _busService.AddBus(bus);
                if (TryCreateNewBus(GetQueuedBusPosition(), false, out BaseBusController incomingBus))
                {
                    _busService.AddBus(incomingBus);
                    
                }
            }
            this.FollowEvent<CreateBusEvent>(OnCreateBusEvent);
        }

        private bool TryCreateNewBus(Vector3 position, bool isArrived, out BaseBusController bus)
        {
            bus = null;
            if (!TryGetBusCreationModel(out BusCreationModel busCreationModel)) return false;
            
            bus = _busFactory.Spawn<StandardBusController>();
            IBaseBusView view = _standardBusViewPool.Spawn<IBaseBusView>(_boardAreaService.BusParent);
            
            BusControllerCreationModel controllerModel = 
                GetUnfinishedBusControllerCreationModel(busCreationModel, isArrived);
            controllerModel.view = view;
            bus.Initialize(controllerModel);
            
            view.SetColor(_colorScriptableObjectsService.GetColor(busCreationModel.BusColor));
            view.SetLocalPosition(position);
            return true;
        }

        private void OnCreateBusEvent(CreateBusEvent obj)
        {
            if (_busService.TryGetBusAtPeek(out BaseBusController bus))
            {
                bus.MoveToBusStop();
                CreateBusDelayed();
            }
            
            // TODO Game WON
            
        }

        private async void CreateBusDelayed()
        {
            await Task.Delay(TimeSpan.FromSeconds(_busCreationDelay));
            if (!TryCreateNewBus(GetNewCreatedBusPosition(), false, out BaseBusController bus)) return;
            bus.MoveToQueuedPosition(GetQueuedBusPosition());
            _busService.AddBus(bus);
        }

        private Vector3 GetNewCreatedBusPosition()
        {
            return Vector3.zero + Vector3.right * _gameConstantsAndPositioningService.GetNewBusXPos();
        }
        
        private Vector3 GetQueuedBusPosition()
        {
            return Vector3.zero + Vector3.right * _gameConstantsAndPositioningService.GetQueuedBusXPos();
        }

        private bool TryGetBusCreationModel(out BusCreationModel model)
        {
            if (_busCreationModels.Count > 0)
            {
                model = _busCreationModels.Pop();
                return true;
            }

            model = new BusCreationModel();
            return false;
        }
        
        private BusControllerCreationModel GetUnfinishedBusControllerCreationModel(
            BusCreationModel model, bool isArrived)
        {
            BusControllerCreationModel controllerCreationModel = new BusControllerCreationModel();
            controllerCreationModel.BusType = model.BusType;
            controllerCreationModel.currentPassengers = 0;
            controllerCreationModel.isArrived = isArrived;
            return controllerCreationModel;
        }

        public void RemoveStandardBus(IBaseBusView view)
        {
            _standardBusViewPool.Despawn(view as StandardBusView);
        }
        
        public Stack<BusCreationModel> GetBusCreationModels()
        {
            return _busCreationModels;
        }
    }
    
    public struct BusCreationModel
    {
        public HumanBusType BusType;
        public CommonColor BusColor;
    }
}