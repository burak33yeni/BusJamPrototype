using System.Collections.Generic;
using _Game._Scripts._BoardArea;
using _Game._Scripts._GameConstants;
using _Game._Scripts._GameScene;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Cells._Factory
{
    public class CellCreator
    {
        [Resolve] private GameConstantsAndPositioningService _gameConstantsAndPositioningService;
        [Resolve] private Level _level;
        [Resolve] private BoardCellPool _boardCellPool;
        [Resolve] private BusStopCellPool _busStopCellPool;
        [Resolve] private BoardAreaService _boardAreaService;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;

        private Dictionary<Vector2Int, BoardCell> _boardCells;
        private Dictionary<int, BusStopCell> _busStopCells;
        
        public void Initialize()
        {
            _boardCells = new Dictionary<Vector2Int, BoardCell>();
            _busStopCells = new Dictionary<int, BusStopCell>();
            CreateBoardCells();
            CreateBusStopCells();
        }

        private void CreateBusStopCells()
        {
            List<BusStopCellAvailability> availability = _level.GetBusStopCellsAvailability();
            for (int j = 0; j < availability.Count; j++)
            {
                BusStopCell cell = 
                    _busStopCellPool.Spawn<BusStopCell>(_boardAreaService.BusStopCellsParent);
                cell.SetPositionAndScale(_gameConstantsAndPositioningService.GetBusStopCellPosition(j));
                cell.SetColorAndText(
                    availability[j].IsAvailable 
                        ? _colorScriptableObjectsService.GetColor(CommonColor.Cell)
                    : _colorScriptableObjectsService.GetColor(CommonColor.NotAvailableCell),
                    availability[j].IsAvailable 
                        ? "" 
                        : "Available at" + availability[j].minLevelToAvailable);
                cell.SetAvailability(availability[j].IsAvailable);
                _busStopCells.Add(j, cell);
            }
        }
        
        public Dictionary<int, bool> GetAvailableBusStopCells()
        {
            Dictionary<int, bool> availableBusStopCells = new Dictionary<int, bool>();
            for (int j = 0; j < _busStopCells.Count; j++)
            {
                if (!_busStopCells.TryGetValue(j, out BusStopCell cell)) continue;
                if (cell == null) continue;
                availableBusStopCells.Add(j, cell.GetAvailability());
            }

            return availableBusStopCells;
        }

        private void CreateBoardCells()
        {
            Vector2Int boardXY = _level.GetBoardXAndY();

            for (int i = 0; i < boardXY.x; i++)
            {
                for (int j = 0; j < boardXY.y; j++)
                {
                    if (!_level.IsCellValid(new Vector2Int(i,j))) continue;
                    BoardCell cell = 
                        _boardCellPool.Spawn<BoardCell>(_boardAreaService.CellsParent);
                    cell.SetPositionAndScale(
                        _gameConstantsAndPositioningService.GetWaitingCellPosition(new Vector2Int(i, j)));
                    cell.SetColor(_colorScriptableObjectsService.GetColor(CommonColor.Cell));
                    _boardCells.Add(new Vector2Int(i, j), cell);
                }
            }
        }
        
        public bool TryGetBoardCellPosition(Vector2Int cellIndex, out Vector3 worldPosition)
        {
            if (_boardCells.ContainsKey(cellIndex))
            {
                worldPosition = _boardCells[cellIndex].GetWorldPosition();
                return true;
            }

            worldPosition = Vector3.zero;
            return false;
        }
        
        public bool TryGetBusStopCellPosition(int cellIndex, out Vector3 worldPosition)
        {
            if (_busStopCells.ContainsKey(cellIndex))
            {
                worldPosition = _busStopCells[cellIndex].GetWorldPosition();
                return true;
            }

            worldPosition = Vector3.zero;
            return false;
        }
    }
}