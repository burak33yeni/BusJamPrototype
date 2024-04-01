using System.Collections.Generic;
using System.Linq;
using _Game._Scripts._BoardArea;
using _Game._Scripts._Buses;
using _Game._Scripts._Buses._BusFactory;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._Human;
using _Game._Scripts._SaveLoad;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GameScene
{
    public class GameSaveLoaderService : IInitializable, IDismissible
    {
        [Resolve] private LevelInventory _levelInventory;
        [Resolve] private MonoActionService _monoActionService;
        [Resolve] private Level _level;
        [Resolve] private BusStopService _busStopService;
        [Resolve] private BusCreator _busCreator;
        [Resolve] private BusService _busService;
        public void Initialize()
        {
            _monoActionService.AddOnApplicationQuitListener(OnApplicationQuit);
        }

        public void Dismiss()
        {
            TrySaveUnfinishedLevel();
            _monoActionService.RemoveOnApplicationQuitListener(OnApplicationQuit);
        }
        
        public bool TryGetPreferredLevelSave(int level, out LevelSaveObject levelSaveObject)
        {
            levelSaveObject = null;
            if (!SaveLoadService.TryLoadLevelFromTxt(level, out LevelSaveObject freshLevel))
                // level not found, go menu to eliminate game crash or stuck in unloaded game screen
                SceneController.LoadEditor();
            else
                levelSaveObject = freshLevel;
            return true;
        }
        
        public bool TryGetNewLevelSave(out LevelSaveObject levelSaveObject, bool lookAtLastPlayed = true)
        {
            levelSaveObject = null;
            int levelItem = _levelInventory.Get<LevelItem>();
            
            if (lookAtLastPlayed)
                if (_levelInventory.Get<LastPlayedLevelItem>() >= levelItem) 
                    return false;
            if (!SaveLoadService.TryLoadLevelFromTxt(levelItem, out LevelSaveObject freshLevel))
            {
                // level not found, go menu to eliminate game crash or stuck in unloaded game screen
                SceneController.LoadStart();
                return false;
            }
            else
                levelSaveObject = freshLevel;
            return true;
        }
        
        public bool TryGetUnfinishedLevelSave(out UnfinishedLevelSaveObject unfinishedLevelSaveObject)
        {
            unfinishedLevelSaveObject = null;
            int levelItem = _levelInventory.Get<LevelItem>();
            if (_levelInventory.Get<LastPlayedLevelItem>() < levelItem) return false;
            if (!SaveLoadService.TryLoadUnfinishedLevelFromTxt(out unfinishedLevelSaveObject))
            {
                EliminateDeadlock();
                // level not found, go menu to eliminate game crash or stuck in unloaded game screen
                SceneController.LoadStart();
                return false;
            }
            else
            {
                if (unfinishedLevelSaveObject.HumansIndexes.Count + 
                    unfinishedLevelSaveObject.HumansIndexesInBusStop.Count <= 0 ||
                    unfinishedLevelSaveObject.CellsIndexes.Count <= 0 ||
                    unfinishedLevelSaveObject.BusesIndexes.Count + 
                    unfinishedLevelSaveObject.CreatedBusesIndexes.Count <= 0 ||
                    unfinishedLevelSaveObject.Level < 0)
                {
                    EliminateDeadlock();
                    return false;
                }
            }
            return true;
        }

        public void EliminateDeadlock()
        {
            // is case of unfinished level save not found, eliminate game stuck in menu
            _levelInventory.Add<LastPlayedLevelItem>(-_levelInventory.Get<LastPlayedLevelItem>());
        }

        private void OnApplicationQuit()
        {
            TrySaveUnfinishedLevel();
        }
        
        private void TrySaveUnfinishedLevel()
        {
            List<Vector2Int> humansIndexes = _level.GetHumansOnBoard().Keys.ToList();
            Dictionary<int, BaseHumanController> humansInBusStop = _busStopService.GetBusStopCellsWithHuman();
            List<int> humanIndexesInBusStop = humansInBusStop.Keys.ToList();
         
            List<HumanBusType> humanTypesInBusStop = humansInBusStop.Values
                .Select(human => human.GetHumanType()).ToList();
            List<HumanBusType> humanTypes = _level.GetHumansOnBoard().Values
                .Select(human => human.GetHumanType()).ToList();
            
            LevelSaveObject levelSaveObject = _level.GetLevelSaveObject();
            UnfinishedLevelSaveObject unfinishedLevelSaveObject = new UnfinishedLevelSaveObject();
            unfinishedLevelSaveObject.Level = levelSaveObject.Level;
            unfinishedLevelSaveObject.BoardX = levelSaveObject.BoardX;
            unfinishedLevelSaveObject.BoardY = levelSaveObject.BoardY;
            unfinishedLevelSaveObject.CellsIndexes = levelSaveObject.CellsIndexes;
            unfinishedLevelSaveObject.CellsValidness = levelSaveObject.CellsValidness;
            
            unfinishedLevelSaveObject.HumansIndexesInBusStop = humanIndexesInBusStop;
            unfinishedLevelSaveObject.HumansTypesInBusStop = humanTypesInBusStop;
            
            unfinishedLevelSaveObject.HumansIndexes = humansIndexes;
            unfinishedLevelSaveObject.HumansTypes = humanTypes;

            Stack<BusCreationModel> busCreationModels = _busCreator.GetBusCreationModels();
            List<int> busIndexes = new List<int>();
            List<HumanBusType> busTypes = new List<HumanBusType>();
            for (int i = 0; i < busCreationModels.Count; i++)
            {
                busIndexes.Add(i);
                busTypes.Add(busCreationModels.ElementAt(i).BusType);
            }
            unfinishedLevelSaveObject.BusesIndexes = busIndexes;
            unfinishedLevelSaveObject.BusesTypes = busTypes;

            Queue<BaseBusController> createdBuses = _busService.GetCreatedBuses();
            int index = 0;
            if (createdBuses.Count > 0)
            {
                unfinishedLevelSaveObject.CreatedBusesIndexes = new List<int>();
                unfinishedLevelSaveObject.CreatedBusesTypes = new List<HumanBusType>();
                foreach (var bus in createdBuses)
                {
                    unfinishedLevelSaveObject.CreatedBusesIndexes.Add(index);
                    unfinishedLevelSaveObject.CreatedBusesTypes.Add(bus.GetBusType());
                    index++;
                }
            }
            
            List<HumanBusType> humansInBus = new List<HumanBusType>();
            if (_busService.TryGetBusAtPeek(out BaseBusController activeBus))
            {
                unfinishedLevelSaveObject.HumansPositionsInBus = activeBus.GetPassengersPositions();
                foreach (var _ in unfinishedLevelSaveObject.HumansPositionsInBus )
                {
                    humansInBus.Add(activeBus.GetBusType());
                }
            }
            
            if (IsHumanColorNumbersNotValid(humanTypes, humanTypesInBusStop, humansInBus)) return;
            
            SaveLoadService.SaveUnfinishedLevelToTxt(unfinishedLevelSaveObject);
            
#if UNITY_EDITOR
            Debug.Log("SaveUnfinishedLevel");
#endif
        }

        private bool IsHumanColorNumbersNotValid(List<HumanBusType> humanTypes, List<HumanBusType> humanTypesInBusStop,
            List<HumanBusType> humansInBus)
        {
            int totalHumanCount = humanTypes.Count + humanTypesInBusStop.Count + humansInBus.Count;
            
            // something can be wrong while saving unfinished level, so check if it is valid
            if (totalHumanCount <= 0 || 
                totalHumanCount % GameConstantsAndPositioningService.StandardBusMaxPassengers != 0) return true;
            
            List<HumanBusType> allHumans = new List<HumanBusType>();
            allHumans.AddRange(humanTypes);
            allHumans.AddRange(humanTypesInBusStop);
            allHumans.AddRange(humansInBus);
            allHumans.Sort( (x, y) => x.CompareTo(y));
            int sameColorCount = 0;
            HumanBusType lastType = HumanBusType.Blue;
            for (int i = 0; i < allHumans.Count; i++)
            {
                if (allHumans[i] == lastType)
                    sameColorCount++;
                else
                {
                    if (sameColorCount % GameConstantsAndPositioningService.StandardBusMaxPassengers != 0)
                    {
                        return true;
                    }
                    sameColorCount = 1;
                    lastType = allHumans[i];
                }
            }
            if (sameColorCount % GameConstantsAndPositioningService.StandardBusMaxPassengers != 0)
            {
                return true;
            }

            return false;
        }
    }
}