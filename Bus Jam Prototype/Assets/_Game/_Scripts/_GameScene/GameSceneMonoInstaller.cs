using _Game._Scripts._BoardArea;
using _Game._Scripts._Buses;
using _Game._Scripts._Buses._BusFactory;
using _Game._Scripts._Buses._Events;
using _Game._Scripts._Cells;
using _Game._Scripts._Cells._Factory;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GameUI;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._Human;
using _Game._Scripts._Human._Events;
using _Game._Scripts._Human._HumanFactory;
using _Game._Scripts._Popups;
using _Game._Scripts._Popups._PopupFactory;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GameScene
{
    public class GameSceneMonoInstaller : MonoInstaller
    {
        [SerializeField] private CanvasService canvasService;
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform poolParent;
        [SerializeField] private BoardAreaService boardAreaService;
        [SerializeField] private PrefabScriptableObjectsService prefabScriptableObjectsService;
        [SerializeField] private ColorScriptableObjectsService colorScriptableObjectsService;
        [SerializeField] private CommonUseColoringService commonUseColoringService;
        [SerializeField] private GameSceneUIView gameSceneUIView;
        [SerializeField] private GameFailPopupView gameFailPopupViewPrefab;

        public override void Install(Context context)
        {
            context.Register<CanvasService>().FromInstance(canvasService);
            context.Register<Canvas>().FromInstance(canvas);
            context.Register<BoardAreaService>().FromInstance(boardAreaService).NonLazy();
            context.Register<CellCreator>().FromNew().NonLazy();
            context.Register<GameInitializer>().FromNew().NonLazy();
            context.Register<GameConstantsAndPositioningService>().FromNew().NonLazy();
            context.Register<Level>().FromNew().NonLazy();
            context.Register<HumanControllerHelper>().FromNew().NonLazy();
            context.Register<ColorScriptableObjectsService>()
                .FromInstance(colorScriptableObjectsService).NonLazy();
            
            context.Register<CommonUseColoringService>().FromInstance(commonUseColoringService).NonLazy();
            context.Register<BusStopService>().FromNew().NonLazy();
            context.RegisterFactory<BaseHumanController, HumanFactory>();
            context.Register<HumanCreator>().FromNew().NonLazy();
            
            RegisterPools(context);

            context.Register<HumanEventHelper>().FromNew().NonLazy();
            RegisterEvents(context);

            context.Register<BusService>().FromNew().NonLazy();
            context.Register<BusCreator>().FromNew().NonLazy();
            context.RegisterFactory<BaseBusController, BusFactory>();
            context.Register<BusEventHelper>().FromNew().NonLazy();

            context.Register<GameSceneUIService>().FromNew().NonLazy()
                .WithArguments(gameSceneUIView);
            context.Register<GameSceneTimerService>().FromNew().NonLazy();

            context.Register<GameSaveLoaderService>().FromNew().NonLazy();
            
            context.Register<LevelInventory>().FromNew();
            context.Register<HealthInventory>().FromNew();
            
            context.RegisterFactory<GameFailPopupView, GameFailPopupFactory>(gameFailPopupViewPrefab);
            context.Register<GameFailPopup>().FromNew().NonLazy();

            context.Register<GameSuccessAndFailService>().FromNew().NonLazy();
        }

        private static void RegisterEvents(Context context)
        {
            context.RegisterEvent<BoardIndexEmptyEvent>();
            context.RegisterEvent<CreateBusEvent>();
            context.RegisterEvent<BusArrivedToBusStopEvent>();
            context.RegisterEvent<CheckGameEndedEvent>();
            context.RegisterEvent<OpenGameFailPopupEvent>();
            context.RegisterEvent<BusLeftBusStopEvent>();
            context.RegisterEvent<KillAllAnimationTweensEvent>();
            context.RegisterEvent<HumanArrivedBusStopEvent>();
        }

        private void RegisterPools(Context context)
        {
            context.RegisterPool<BoardCell, BoardCellPool>(GetPrefab<BoardCell>(PrefabType.WaitingCell))
                .WithParent(poolParent).WithMinimumCount(100);
            context.RegisterPool<BusStopCell, BusStopCellPool>(GetPrefab<BusStopCell>(PrefabType.BusStopCell))
                .WithMinimumCount(10).WithParent(poolParent);
            context.RegisterPool<HumanView, HumanViewPool>(GetPrefab<HumanView>(PrefabType.HumanView))
                .WithMinimumCount(15).WithParent(poolParent);
            context.RegisterPool<StandardBusView, StandardBusViewPool>(GetPrefab<StandardBusView>(PrefabType.BusView))
                .WithMinimumCount(5).WithParent(poolParent);
        }

        private TComponent GetPrefab<TComponent>(PrefabType prefabType) where TComponent : Component
        {
            return prefabScriptableObjectsService.GetPrefab<TComponent>(prefabType);
        }
    }
}
