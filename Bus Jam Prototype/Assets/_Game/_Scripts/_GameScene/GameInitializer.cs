using System.Collections.Generic;
using _Game._Scripts._BoardArea;
using _Game._Scripts._Buses;
using _Game._Scripts._Buses._BusFactory;
using _Game._Scripts._Cells._Factory;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._Human;
using _Game._Scripts._Human._HumanFactory;
using _Game._Scripts._SaveLoad;
using _Game._Scripts._ScriptableObjects;
using _Game._Scripts.LevelEditor;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GameScene
{
    public class GameInitializer : IInitializable
    {
        private static bool _testMode = false;
        public static bool TestMode
        {
            get => _testMode;
            set => _testMode = value;
        }

        [Resolve] private Level _level;
        [Resolve] private GameConstantsAndPositioningService _gameConstantsAndPositioningService;
        [Resolve] private CellCreator _cellCreator;
        [Resolve] private HumanCreator _humanCreator;
        [Resolve] private BusStopService _busStopService;
        [Resolve] private BusCreator _busCreator;
        [Resolve] private BusService _busService;
        [Resolve] private LevelInventory _levelInventory;
        [Resolve] private GameSaveLoaderService _gameSaveLoaderService;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        
        public void Initialize()
        {
            bool freshSave = false;
            LevelSaveObject freshSaveFile = null;
            UnfinishedLevelSaveObject unfinishedSaveFile = null;
            
#if UNITY_EDITOR
            if (PlayerPrefs.GetInt(LevelEditorSaveLoadService.QuickTestKey, 0) == 1)
            {
                PlayerPrefs.SetInt(LevelEditorSaveLoadService.QuickTestKey, 0);
                if (_gameSaveLoaderService.TryGetPreferredLevelSave(-1, out var preferredSave))
                {
                    _testMode = true;
                    freshSave = true;
                    freshSaveFile = preferredSave;
                }
            }    
#endif
            if (!_testMode)
            {
                if (_gameSaveLoaderService.TryGetNewLevelSave(out LevelSaveObject save))
                {
                    freshSaveFile = save;
                    freshSave = true;
                }
                else if (_gameSaveLoaderService.TryGetUnfinishedLevelSave(
                             out UnfinishedLevelSaveObject unfinishedSave))
                {
                    unfinishedSaveFile = unfinishedSave;
                    freshSave = false;
                }
                else
                {
                    _gameSaveLoaderService.EliminateDeadlock();
                    _gameSaveLoaderService.TryGetNewLevelSave(
                        out LevelSaveObject saveForced, false);
                    freshSaveFile = saveForced;
                    freshSave = true;
                }
            }
            
            if (freshSave)
                _level.Initialize(freshSaveFile);
            else
                _level.Initialize(unfinishedSaveFile);
                
            _gameConstantsAndPositioningService.SetXRowCountIsOddOrEven(_level.GetXRowCountIsOddOrEven());
            _cellCreator.Initialize();
            
            Dictionary<Vector2Int, BaseHumanController> humansAtStart = 
                _humanCreator.SpawnHumansAtStart(_level.GetHumansModelsAtStart());
            _level.SetHumans(humansAtStart);
            _level.SetMovementIndicatorsOfHumans();
            
            _busStopService.Initialize();
            if (!freshSave)
                _busStopService.LoadFromSave(unfinishedSaveFile);
            
            _busService.Initialize();
            _busCreator.Initialize(_level.GetBusCreationModels());

            if (!freshSave)
            {
                if (_busService.TryGetBusAtPeek(out BaseBusController bus))
                {
                    for (int i = 0; i < unfinishedSaveFile.HumansPositionsInBus.Count; i++)
                    {
                        bus.TryAddPassenger(out Vector3 position);
                        BaseHumanController human = _humanCreator.SpawnHumanInBus(
                            Vector2Int.zero, position, 
                            _colorScriptableObjectsService.GetColor(
                                bus.GetBusType()), bus.GetBusType());
                        bus.SetPassengerParentWhenPassengerArrived(human.GetView());
                        
                    }
                }
            }

            _busStopService.TrySendHumansToBusAndCheckGameEnded();
            if (!_testMode)
            {
                _levelInventory.Add<LastPlayedLevelItem>(-_levelInventory.Get<LastPlayedLevelItem>());
                _levelInventory.Add<LastPlayedLevelItem>(_levelInventory.Get<LevelItem>());   
            }
        }
    }
}