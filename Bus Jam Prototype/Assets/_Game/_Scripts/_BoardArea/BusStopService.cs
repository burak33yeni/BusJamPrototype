using System.Collections.Generic;
using System.Linq;
using _Game._Scripts._Buses;
using _Game._Scripts._Buses._Events;
using _Game._Scripts._Cells._Factory;
using _Game._Scripts._GameScene;
using _Game._Scripts._Human;
using _Game._Scripts._Human._Events;
using _Game._Scripts._Human._HumanFactory;
using _Game._Scripts._SaveLoad;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using TMPro;
using UnityEngine;

namespace _Game._Scripts._BoardArea
{
    public class BusStopService
    {
        [Resolve] private Level _level;
        [Resolve] private CellCreator _cellCreator;
        [Resolve] private BusService _busService;
        [Resolve] private HumanCreator _humanCreator;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        
        private Dictionary<int, bool> _busStopCells; // index, isAvailableInThisLevel
        private Dictionary<int, BaseHumanController> _busStopCellsWithHuman; // index, humanController
       
        public void Initialize()
        {
            _busStopCells = _cellCreator.GetAvailableBusStopCells();
            _busStopCellsWithHuman = new Dictionary<int, BaseHumanController>();
            this.FollowEvent<BusArrivedToBusStopEvent>(OnBusArrivedToBusStopEvent);
            this.FollowEvent<HumanArrivedBusStopEvent>(OnHumanArrivedBusStopEvent);
        }

        private void OnHumanArrivedBusStopEvent(HumanArrivedBusStopEvent obj)
        {
            TrySendHumansToBusAndCheckGameEnded();
        }

        public void LoadFromSave(UnfinishedLevelSaveObject unfinishedSave)
        {
            for (var i = 0; i < unfinishedSave.HumansIndexesInBusStop.Count; i++)
            {
                var humanIndex = unfinishedSave.HumansIndexesInBusStop[i];
                _cellCreator.TryGetBusStopCellPosition(humanIndex, out Vector3 position);
                BaseHumanController human = _humanCreator.SpawnHumanAtBusStop(Vector2Int.zero, position,
                    _colorScriptableObjectsService.GetColor(
                        unfinishedSave.HumansTypesInBusStop[i]),
                    unfinishedSave.HumansTypesInBusStop[i]);
                if (_busStopCells.ContainsKey(humanIndex))
                    _busStopCells[humanIndex] = true;
                _busStopCellsWithHuman.Add(humanIndex, human);
            }
        }

        private void OnBusArrivedToBusStopEvent(BusArrivedToBusStopEvent obj)
        {
            TrySendHumansToBusAndCheckGameEnded();
        }

        public void TrySendHumansToBusAndCheckGameEnded()
        {
            if (TrySendHumansToBus())
                this.SendEvent(new CheckGameEndedEvent());
        }

        private bool TrySendHumansToBus()
        {
            bool sendEvent = true;
            List<int> indexWentBus = new List<int>();
            foreach (var keyValuePair in _busStopCellsWithHuman)
            {
                if (keyValuePair.Value.IsMoving())
                    sendEvent = false;
                if (!keyValuePair.Value.AutoTriggerToGoBusInBusStop()) continue;
                indexWentBus.Add(keyValuePair.Key);
            }

            foreach(var item in indexWentBus)
                _busStopCellsWithHuman.Remove(item);
            return sendEvent;
        }

        public bool TryGetAvailableBusStopCell(out int index)
        {
            foreach (var busStopCell in _busStopCells)
            {
                if (!busStopCell.Value) continue;
                if (_busStopCellsWithHuman.ContainsKey(busStopCell.Key)) continue;
                index = busStopCell.Key;
                return true;
            }
            index = -1;
            return false;
        }

        public bool TryAddHumanToCell(BaseHumanController human, int busStopIndex)
        {
            if (_busStopCellsWithHuman.ContainsKey(busStopIndex)) return false;
            if (_busStopCells.ContainsKey(busStopIndex) && !_busStopCells[busStopIndex]) return false;
            _busStopCellsWithHuman.Add(busStopIndex, human);
            return true;
        }
        
        public Dictionary<int, BaseHumanController> GetBusStopCellsWithHuman()
        {
            return _busStopCellsWithHuman;
        }
        
        public bool IsBusStopFull()
        {
            int availableBusStopCells = _busStopCells.Count(kvp => kvp.Value);
            return availableBusStopCells <= _busStopCellsWithHuman.Count;
        }
        
        public int GetHumansCountInBusStop()
        {
            return _busStopCellsWithHuman.Count;
        }
    }
    
    public struct BusStopCellAvailability
    {
        public int Index;
        public bool IsAvailable;
        public int minLevelToAvailable;
    }
}