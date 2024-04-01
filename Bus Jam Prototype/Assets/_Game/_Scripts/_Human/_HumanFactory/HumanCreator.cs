using System.Collections.Generic;
using _Game._Scripts._BoardArea;
using _Game._Scripts._GameConstants;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Human._HumanFactory
{
    public class HumanCreator
    {
        [Resolve] private HumanFactory _humanFactory;
        [Resolve] private HumanViewPool _humanViewPool;
        [Resolve] private BoardAreaService _boardAreaService;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        [Resolve] private GameConstantsAndPositioningService _gameConstantsAndPositioningService;
        
        public Dictionary<Vector2Int, BaseHumanController> SpawnHumansAtStart(List<HumanCreationModel> humans)
        {
            Dictionary<Vector2Int, BaseHumanController> humanControllers =
                new Dictionary<Vector2Int, BaseHumanController>();
            foreach (var human in humans)
            {
                humanControllers.Add(human.starterIndex, GetHumanController(human));
            }
            return humanControllers;
        }
        
        public BaseHumanController GetHumanController(HumanCreationModel model)
        {
            BaseHumanController humanController = GetHumanController();
            IHumanView view = GetHumanView();
            model.view = view;
            humanController.Initialize(model);
            return humanController;
        }
        
        public HumanCreationModel GetHumanCreationModel(Vector2Int starterIndex, 
            CommonColor commonColor, HumanBusType busType, HumanPosition position)
        {
            return new HumanCreationModel(null, starterIndex, commonColor,
                _colorScriptableObjectsService.GetColor(commonColor), 
                _gameConstantsAndPositioningService.GetHumanPosition(starterIndex),
                busType, position);
        }

        private IHumanView GetHumanView()
        {
            return _humanViewPool.Spawn<IHumanView>(_boardAreaService.HumanParent);
        }
        
        public void RemoveHumanView(HumanView humanView)
        {
            _humanViewPool.Despawn(humanView);
        }

        private BaseHumanController GetHumanController()
        {
            return _humanFactory.Spawn<HumanController>();
        }
        
        public BaseHumanController SpawnHumanAtBusStop(Vector2Int starterIndex, Vector3 position, CommonColor commonColor, HumanBusType busType)
        {
            BaseHumanController humanController = GetHumanController();
            IHumanView view = GetHumanView();
            HumanCreationModel model = new HumanCreationModel(view, starterIndex, commonColor,
                _colorScriptableObjectsService.GetColor(commonColor),
                position, busType, HumanPosition.BusStopCell);
            humanController.Initialize(model);
            return humanController;
        }
        
        public BaseHumanController SpawnHumanInBus(Vector2Int starterIndex, Vector3 position, CommonColor commonColor, HumanBusType busType)
        {
            BaseHumanController humanController = GetHumanController();
            IHumanView view = GetHumanView();
            HumanCreationModel model = new HumanCreationModel(view, starterIndex, commonColor,
                _colorScriptableObjectsService.GetColor(commonColor),
                position, busType, HumanPosition.InBus);
            humanController.Initialize(model);
            return humanController;
        }
    }
}