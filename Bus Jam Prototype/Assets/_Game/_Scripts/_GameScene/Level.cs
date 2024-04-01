using System.Collections.Generic;
using _Game._Scripts._BoardArea;
using _Game._Scripts._Buses._BusFactory;
using _Game._Scripts._GameConstants;
using _Game._Scripts._Human;
using _Game._Scripts._Human._HumanFactory;
using _Game._Scripts._SaveLoad;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GameScene
{
    public class Level
    {
        [Resolve] private HumanCreator _humanCreator;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        
        private Vector2Int _boardXY;
        private bool[,] _boardCells;
        private Dictionary<Vector2Int, BaseHumanController> _humansOnBoard;
        private LevelSaveObject _levelSaveObject;
        
        public void Initialize(LevelSaveObject levelSaveObject)
        {
            _levelSaveObject = levelSaveObject;
            _humansOnBoard = new Dictionary<Vector2Int, BaseHumanController>();
            _boardXY = new Vector2Int(levelSaveObject.BoardX, levelSaveObject.BoardY);
            
            SetCells(levelSaveObject);
        }

        private void SetCells(LevelSaveObject levelSaveObject)
        {
            _boardCells = new bool[_boardXY.x, _boardXY.y];
            for (int i = 0; i < levelSaveObject.CellsValidness.Count; i++)
            {
                if (i >= _boardXY.x * _boardXY.y) break;
                _boardCells[levelSaveObject.CellsIndexes[i].x, levelSaveObject.CellsIndexes[i].y]
                    = levelSaveObject.CellsValidness[i];
            }
        }

        public void Initialize(UnfinishedLevelSaveObject unfinishedLevelSaveObject)
        {
            _levelSaveObject = new LevelSaveObject();
            _levelSaveObject.Level = unfinishedLevelSaveObject.Level;
            _levelSaveObject.BoardX = unfinishedLevelSaveObject.BoardX;
            _levelSaveObject.BoardY = unfinishedLevelSaveObject.BoardY;
            _levelSaveObject.CellsIndexes = unfinishedLevelSaveObject.CellsIndexes;
            _levelSaveObject.CellsValidness = unfinishedLevelSaveObject.CellsValidness;
            _levelSaveObject.HumansIndexes = unfinishedLevelSaveObject.HumansIndexes;
            _levelSaveObject.HumansTypes = unfinishedLevelSaveObject.HumansTypes;
            _levelSaveObject.BusesIndexes = unfinishedLevelSaveObject.BusesIndexes;
            _levelSaveObject.BusesTypes = unfinishedLevelSaveObject.BusesTypes;
            unfinishedLevelSaveObject.CreatedBusesTypes.AddRange(_levelSaveObject.BusesTypes);
            unfinishedLevelSaveObject.CreatedBusesIndexes.AddRange(_levelSaveObject.BusesIndexes);
            _levelSaveObject.BusesTypes = unfinishedLevelSaveObject.CreatedBusesTypes;
            _levelSaveObject.BusesIndexes = unfinishedLevelSaveObject.CreatedBusesIndexes;
            
            
            _humansOnBoard = new Dictionary<Vector2Int, BaseHumanController>();
            _boardXY = new Vector2Int(_levelSaveObject.BoardX, _levelSaveObject.BoardY);
            
            SetCells(_levelSaveObject);
        }
        
        public bool GetXRowCountIsOddOrEven()
        {
            return _boardXY.x % 2 == 0;
        }
        
        public Vector2Int GetBoardXAndY()
        {
            return _boardXY;
        }
        
        public bool IsCellValid(Vector2Int cellIndex)
        {
            if (cellIndex.x < 0 || cellIndex.x >= GetBoardXAndY().x ||
                cellIndex.y < 0 || cellIndex.y >= GetBoardXAndY().y)
            {
                return false;
            }
            return _boardCells[cellIndex.x, cellIndex.y];
        }
        
        public List<HumanCreationModel> GetHumansModelsAtStart()
        {
            List<HumanCreationModel> humans = new List<HumanCreationModel>();
            for (int i = 0; i < _levelSaveObject.HumansIndexes.Count; i++)
            {
                humans.Add(
                    _humanCreator.GetHumanCreationModel(
                        new Vector2Int(
                            _levelSaveObject.HumansIndexes[i].x,
                            _levelSaveObject.HumansIndexes[i].y),
                        _colorScriptableObjectsService.GetColor(_levelSaveObject.HumansTypes[i]),
                        _levelSaveObject.HumansTypes[i], HumanPosition.WaitingCell));
            }
            return humans;
        }
        
        public void SetHumans(Dictionary<Vector2Int, BaseHumanController> humans)
        {
            _humansOnBoard = humans;
        }

        public void SetMovementIndicatorsOfHumans()
        {
            foreach (var baseHumanController in _humansOnBoard)
            {
                baseHumanController.Value.SetCanMoveNow();
            }
        }
        
        public bool IsCellAvailableToMove(Vector2Int cellIndex)
        {
            if (cellIndex.x < 0 || cellIndex.x >= GetBoardXAndY().x ||
                cellIndex.y < 0 || cellIndex.y >= GetBoardXAndY().y)
            {
                return false;
            }
            return _boardCells[cellIndex.x, cellIndex.y] && 
                   !_humansOnBoard.ContainsKey(cellIndex);
        }
        
        public bool IsThisTheCellToMoveBusStopCells(Vector2Int cellIndex)
        {
            return cellIndex.y == GetBoardXAndY().y - 1 &&
                   cellIndex.x >= 0 && cellIndex.x < GetBoardXAndY().x &&
                   _boardCells[cellIndex.x, cellIndex.y];
        }

        public List<BusStopCellAvailability> GetBusStopCellsAvailability()
        {
            int maxCount = GameConstantsAndPositioningService.GetMaxBusStopCellCount();
            List<BusStopCellAvailability> cells = new List<BusStopCellAvailability>();
            for (int i = 0; i < maxCount; i++)
            {
                cells.Add(new BusStopCellAvailability
                {
                    Index = i,
                    IsAvailable = IsBusStopCellAvailable(i, out int minLevelToAvailable),
                    minLevelToAvailable = minLevelToAvailable
                });
            }
            return cells;
        }
        
        private bool IsBusStopCellAvailable(int index, out int minLevelToAvailable)
        {
            switch (index)
            {
                case 0:
                    minLevelToAvailable =
                        GameConstantsAndPositioningService.FirstBusStopCellUnlockedLevel;
                    return 
                        _levelSaveObject.Level >= GameConstantsAndPositioningService.FirstBusStopCellUnlockedLevel;
                case 6:
                    minLevelToAvailable = 
                        GameConstantsAndPositioningService.LastBusStopCellUnlockedLevel;
                    return 
                        _levelSaveObject.Level >= GameConstantsAndPositioningService.LastBusStopCellUnlockedLevel;
                default:
                    minLevelToAvailable = 0;
                    return true;
            }
        }
        
        public void RemoveHumanOnBoardCell(Vector2Int cellIndex)
        {
            if (!_humansOnBoard.ContainsKey(cellIndex)) return;
            _humansOnBoard.Remove(cellIndex);
        }
        
        public void AddHumanOnBoardCell(Vector2Int cellIndex, BaseHumanController human)
        {
            if (_humansOnBoard.ContainsKey(cellIndex)) return;
            _humansOnBoard.Add(cellIndex, human);
        }
        
        public Stack<BusCreationModel> GetBusCreationModels()
        {
            Stack<BusCreationModel> busCreationModels = new Stack<BusCreationModel>();
            
            for (int i = _levelSaveObject.BusesIndexes.Count-1; i >= 0 ; i--)
            {
                busCreationModels.Push(new BusCreationModel
                {
                    BusType = _levelSaveObject.BusesTypes[i],
                    BusColor = _colorScriptableObjectsService.GetColor(_levelSaveObject.BusesTypes[i])
                });
            }
            
            return busCreationModels;
        }
        
        public string GetLevelName()
        {
            return _levelSaveObject.Level.ToString();
        }
        
        public LevelSaveObject GetLevelSaveObject()
        {
            return _levelSaveObject;
        }
        
        public Dictionary<Vector2Int, BaseHumanController> GetHumansOnBoard()
        {
            return _humansOnBoard;
        } 
        
        public bool IsThereAnyHumanOnBoard()
        {
            return _humansOnBoard.Count > 0;
        }
    }
}