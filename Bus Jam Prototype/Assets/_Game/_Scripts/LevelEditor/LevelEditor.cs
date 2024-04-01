using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Game._Scripts._GameConstants;
using _Game._Scripts._SaveLoad;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.LevelEditor
{
    public class LevelEditor : MonoBehaviour, IInitializable
    {
        private static readonly float _cellDiff = 35f;
        private static readonly float _cellSize = 30f;
        private static readonly int _busSeatCount = 3;
        private static readonly int _minHumanCount = 3;
        
        [Resolve] private CanvasService _canvasService;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService; 
        [SerializeField] private HumanForEditor _humanForEditorPrefab;
        [SerializeField] private DeletedCellForEditor _deletedCellForEditorPrefab;
        [SerializeField] private CellForEditor _cellForEditorPrefab;
        [SerializeField] private BusForEditor _busForEditorPrefab;
        
        [SerializeField] private TextMeshProUGUI _humanCountText, _busCountText;
        
        [SerializeField] private RectTransform boardArea, roadArea, cellHolder;
        public RectTransform CellHolder => cellHolder;
        public DraggableEditorObject DraggableEditorObject;
        [SerializeField] private Button Xadd, Xremove, Yadd, Yremove;

        #region EditorColorButtons

        [SerializeField] private HumanButtonBlueForEditor _blueButton;
        [SerializeField] private HumanButtonGreenForEditor _greenButton;
        [SerializeField] private HumanButtonRedForEditor _redButton;
        [SerializeField] private HumanButtonYellowForEditor _yellowButton;
        [SerializeField] private HumanButtonMagentaForEditor _magentaButton;
        [SerializeField] private HumanButtonOrangeForEditor _orangeButton;

        #endregion
        #region Data

        private Dictionary<Vector2Int, bool> _cells;
        private Dictionary<Vector2Int, BaseCellForEditor> _cellObjects;
        
        private Dictionary<Vector2Int, HumanForEditor> _humans;
        private Dictionary<Vector2Int, HumanBusType> _humanBusTypes;
        
        private Dictionary<int, BusForEditor> _buses;
        private Dictionary<int, HumanBusType> _busTypes;
        
        private Dictionary<int, BaseCellForEditor> _busCellObjects;
        private int _currX, _currY;

        #endregion
        public void Initialize()
        {
            SetButtonColors();

            _cells = new Dictionary<Vector2Int, bool>();
            _cellObjects = new Dictionary<Vector2Int, BaseCellForEditor>();
            _humans = new Dictionary<Vector2Int, HumanForEditor>();
            _humanBusTypes = new Dictionary<Vector2Int, HumanBusType>();
            _buses = new Dictionary<int, BusForEditor>();
            _busTypes = new Dictionary<int, HumanBusType>();
            _busCellObjects = new Dictionary<int, BaseCellForEditor>();
            AddNewCell( 0, 0, 0, 0);
            _currX = 1;
            _currY = 1;
            AdjustCellHolder();
            AddRemoveBoardXY();
            _humanCountText.SetText("Human Count must be multiple of " + _busSeatCount);
            
        }

        private void SetButtonColors()
        {
            _blueButton.Init(_colorScriptableObjectsService.GetColor(CommonColor.Blue));
            _greenButton.Init(_colorScriptableObjectsService.GetColor(CommonColor.Green));
            _redButton.Init(_colorScriptableObjectsService.GetColor(CommonColor.Red));
            _yellowButton.Init(_colorScriptableObjectsService.GetColor(CommonColor.Yellow));
            _magentaButton.Init(_colorScriptableObjectsService.GetColor(CommonColor.Magenta));
            _orangeButton.Init(_colorScriptableObjectsService.GetColor(CommonColor.Orange));
        }

        private void AddRemoveBoardXY()
        {
            Xadd.onClick.AddListener(() =>
            {
                if (GameConstantsAndPositioningService._maxBoardCellCountOnAxis <= _currX) return;
                for (int i = 0; i < _currY; i++)
                {
                    AddNewCell( _currX, i, _currX * _cellDiff, i * _cellDiff);
                }
                _currX++;
                AdjustCellHolder();
            });
            
            Yadd.onClick.AddListener(() =>
            {
                if (GameConstantsAndPositioningService._maxBoardCellCountOnAxis <= _currY) return;
                for (int i = 0; i < _currX; i++)
                {
                    AddNewCell(i, _currY, i * _cellDiff, _currY * _cellDiff);
                }
                _currY++;
                AdjustCellHolder();
            });
            
            Xremove.onClick.AddListener(() =>
            {
                if (_currX <= 1) return;
                for (int i = 0; i < _currY; i++)
                {
                    RemoveCell(_currX - 1, i);
                }
                _currX--;
                AdjustCellHolder();
            });
            
            Yremove.onClick.AddListener(() =>
            {
                if (_currY <= 1) return;
                for (int i = 0; i < _currX; i++)
                {
                    RemoveCell(i, _currY - 1);
                }
                _currY--;
                AdjustCellHolder();
            });
        }

        private void AddNewCell(int xIndex, int yIndex, float xPos, float yPos)
        {
            _cells.Add(new Vector2Int(xIndex, yIndex), true);
            AddCellObject(xIndex, yIndex, xPos, yPos);
        }

        private void AddCellObject(int xIndex, int yIndex, float xPos, float yPos)
        {
            var newCell = Instantiate(_cellForEditorPrefab, boardArea);
            newCell.transform.localPosition = new Vector3(xPos, yPos, 0);
            _cellObjects.Add(new Vector2Int(xIndex, yIndex), newCell);
            newCell.AddDeleteButtonListener(() =>
            {
                OnCellClicked(xIndex, yIndex);
            });
        }
        
        private void AddDeletedCellObject(int xIndex, int yIndex, float xPos, float yPos)
        {
            var newCell = Instantiate(_deletedCellForEditorPrefab, boardArea);
            newCell.transform.localPosition = new Vector3(xPos, yPos, 0);
            _cellObjects.Add(new Vector2Int(xIndex, yIndex), newCell);
            newCell.AddDeleteButtonListener(() =>
            {
                OnCellClicked(xIndex, yIndex);
            });
        }

        private void RemoveCell(int xIndex, int yIndex)
        {
            if (_cells.Count <= 1) return;
            if (_humans.ContainsKey(new Vector2Int(xIndex, yIndex)))
                RemoveHuman(new Vector2Int(xIndex, yIndex));
            
            _cells.Remove(new Vector2Int(xIndex, yIndex));
            RemoveCellObject(xIndex, yIndex);
        }

        private void RemoveCellObject(int xIndex, int yIndex)
        {
            Destroy(_cellObjects[new Vector2Int(xIndex, yIndex)].gameObject);
            _cellObjects.Remove(new Vector2Int(xIndex, yIndex));
        }

        private void ReplaceCell(int xIndex, int yIndex)
        {
            if (_cellObjects[new Vector2Int(xIndex, yIndex)].GetType() == typeof(CellForEditor))
            {
                _cells[new Vector2Int(xIndex, yIndex)] = false;
                Vector3 pos = _cellObjects[new Vector2Int(xIndex, yIndex)].transform.localPosition;
                RemoveCellObject(xIndex, yIndex);
                AddDeletedCellObject(xIndex, yIndex, pos.x, pos.y);
            }
            else if (_cellObjects[new Vector2Int(xIndex, yIndex)].GetType() == typeof(DeletedCellForEditor))
            {
                _cells[new Vector2Int(xIndex, yIndex)] = true;
                Vector3 pos = _cellObjects[new Vector2Int(xIndex, yIndex)].transform.localPosition;
                RemoveCellObject(xIndex, yIndex);
                AddCellObject(xIndex, yIndex, pos.x, pos.y);
            }
            

        }

        private void AdjustCellHolder()
        {
            cellHolder.localPosition = new Vector3(
                -1 * (_currX - 1) * _cellDiff / 2, 
                -1 * (_cellSize / 2f) - (_currY-1) * _cellDiff,
                0);
        }

        private void OnCellClicked(int x, int y)
        {
            ReplaceCell(x, y);
        }
        
        public void AddHumanOrBus(Vector3 position, HumanBusType humanBusType, CommonColor co)
        {
            if (!TryGetIndexFromPosition(position, out Vector2Int cellIndex, out bool isBus)) return;
            if (!isBus && _humans.ContainsKey(cellIndex)) return;
            if (isBus && _buses.ContainsKey(cellIndex.x)) return;
            if (!isBus)
            {
                HumanForEditor human = Instantiate(_humanForEditorPrefab, cellHolder);
                human.transform.position = _cellObjects[cellIndex].transform.position;
                human.humanBusType = humanBusType;
                human.Init(_colorScriptableObjectsService.GetColor(co));
                _humans.Add(cellIndex, human);
                if (!_humanBusTypes.ContainsKey(cellIndex))
                    _humanBusTypes.Add(cellIndex, humanBusType);
                PrintHumans();
                human.AddDeleteButtonListener(() =>
                {
                    RemoveHuman(cellIndex);
                });
                AddBusCell(_humanBusTypes.Count);
            }
            else
            {
                BusForEditor bus = Instantiate(_busForEditorPrefab, _busCellObjects[cellIndex.x].transform);
                bus.transform.localPosition = Vector3.zero;
                bus.HumanBusType = humanBusType;
                bus.Init(_colorScriptableObjectsService.GetColor(co));
                _buses.Add(cellIndex.x, bus);
                if (!_busTypes.ContainsKey(cellIndex.x))
                    _busTypes.Add(cellIndex.x, humanBusType);
                bus.AddDeleteButtonListener( () =>
                {
                    RemoveBus(cellIndex);
                });
                PrintBuses();
            }
           
        }

        private void AddBusCell(int humanCount)
        {
            int requiredBusCount = humanCount / _busSeatCount;
            if (requiredBusCount <= _busCellObjects.Count) return;

            while (requiredBusCount > _busCellObjects.Count)
            {
                int index = _busCellObjects.Count;
                var newCell = Instantiate(_cellForEditorPrefab, roadArea);
                newCell.transform.localPosition = new Vector3(index * _cellDiff, 0, 0);
                newCell.transform.SetAsFirstSibling();
                if (!_busCellObjects.ContainsKey(index))
                    _busCellObjects.Add(index, newCell);
                PrintBuses();
            }
            
        }

        private void RemoveBusCell(int humanCount)
        {
            if (humanCount / _busSeatCount >= _busCellObjects.Count) return;
            int index = humanCount / _busSeatCount;
            Destroy(_busCellObjects[index].gameObject);
            _busCellObjects.Remove(index);
            _buses.Remove(index);
            _busTypes.Remove(index);
            PrintBuses();
        }

        private void RemoveBus(Vector2Int cellIndex)
        {
            if (!_buses.ContainsKey(cellIndex.x)) return;
            Destroy(_buses[cellIndex.x].gameObject);
            _buses.Remove(cellIndex.x);
            _busTypes.Remove(cellIndex.x);
            PrintBuses();
        }

        private void RemoveHuman(Vector2Int cellIndex)
        {
            if (!_humans.ContainsKey(cellIndex)) return;
            Destroy(_humans[cellIndex].gameObject);
            _humans.Remove(cellIndex);
            _humanBusTypes.Remove(cellIndex);
            PrintHumans();
            RemoveBusCell(_humanBusTypes.Count);
        }
        
        private void PrintHumans()
        {
            List<HumanForEditor> humans = _humans.Values.ToList().OrderBy(h => h.humanBusType).ToList();
            StringBuilder sb = new StringBuilder();
            int i = 0;
            HumanBusType lastType = HumanBusType.Blue;
            string lastTypeString = "";
            foreach (var singleOne in humans)
            {
                if (lastType == singleOne.humanBusType)
                {
                    i++;
                        lastTypeString = (i % _busSeatCount == 0 ? "<color=white>" : "<color=black>") +
                                          i + " " + singleOne.humanBusType;
                }
                else
                {
                    lastType = singleOne.humanBusType;
                    sb.Append(lastTypeString + System.Environment.NewLine);
                    i = 1;
                    lastTypeString = (i % _busSeatCount == 0 ? "<color=white>" : "<color=black>") +
                                    i + " " + singleOne.humanBusType;
                }
            }
            sb.Append(lastTypeString + System.Environment.NewLine);
            sb.Append(_humans.Count % _busSeatCount == 0 ? "<color=white>" : "<color=black>");
            if (_humans.Count % _busSeatCount == 0)
                sb.Append("#" + _humans.Count + " ok");
            else
                sb.Append("# " + _humans.Count + " NOT ok");
            _humanCountText.SetText(sb);
        }
        
        private void PrintBuses()
        {
            if (_busCellObjects.Count == _buses.Count)
                _busCountText.SetText(_busCellObjects.Count + " Bus is suitable.");
            else
                _busCountText.SetText(_busCellObjects.Count + " Bus must be in level." +
                                      _buses.Count + " Bus is not enough.");
        }

        
        private bool TryGetIndexFromPosition(Vector3 position, out Vector2Int cellIndex,
            out bool isBus)
        {
            float canvasScale = _canvasService.GetScaleFactor();
            foreach (var VARIABLE in _cellObjects)
            {
                if (_cellObjects[VARIABLE.Key].GetType() == typeof(DeletedCellForEditor)) continue;
                if (Mathf.Abs(position.x - VARIABLE.Value.transform.position.x) <= _cellSize * canvasScale / 2f &&
                    Mathf.Abs(position.y - VARIABLE.Value.transform.position.y) <= _cellSize * canvasScale / 2f)
                {
                    cellIndex = VARIABLE.Key;
                    isBus = false;
                    return true;
                }
            }

            foreach (var VARIABLE in _busCellObjects)
            {
                if (Mathf.Abs(position.x - VARIABLE.Value.transform.position.x) <= _cellSize * canvasScale / 2f &&
                    Mathf.Abs(position.y - VARIABLE.Value.transform.position.y) <= _cellSize * canvasScale / 2f)
                {
                    isBus = true;
                    cellIndex = new Vector2Int(VARIABLE.Key, 0);
                    return true;
                }
            }
            isBus = false;
            cellIndex = new Vector2Int();
            return false;
        }
        
        public void LoadLevel(LevelSaveObject levelSaveObject)
        {
            DestroyExistingObjects();
            
            _cells = levelSaveObject.CellsIndexes.Zip(
                    levelSaveObject.CellsValidness, (a, b) => new {a, b})
                .ToDictionary(x => x.a, x => x.b);
            _cellObjects = new Dictionary<Vector2Int, BaseCellForEditor>();
            
            _humanBusTypes = levelSaveObject.HumansIndexes.Zip(
                    levelSaveObject.HumansTypes, (a, b) => new {a, b})
                .ToDictionary(x => x.a, x => x.b);
            _humans = new Dictionary<Vector2Int, HumanForEditor>();

            _busTypes = levelSaveObject.BusesIndexes.Zip(
                    levelSaveObject.BusesTypes, (a, b) => new {a, b})
                .ToDictionary(x => x.a, x => x.b);
            _buses = new Dictionary<int, BusForEditor>();
            _busCellObjects = new Dictionary<int, BaseCellForEditor>();
            
            _currX = levelSaveObject.BoardX;
            _currY = levelSaveObject.BoardY;
            AdjustCellHolder();
            foreach (var VARIABLE in _cells)
            {
                if (VARIABLE.Value)
                    AddCellObject(VARIABLE.Key.x, VARIABLE.Key.y, VARIABLE.Key.x * _cellDiff, VARIABLE.Key.y * _cellDiff);
                else
                    AddDeletedCellObject(VARIABLE.Key.x, VARIABLE.Key.y, VARIABLE.Key.x * _cellDiff, VARIABLE.Key.y * _cellDiff);
            }

            foreach (var VARIABLE in _humanBusTypes)
            {
                AddHumanOrBus(_cellObjects[VARIABLE.Key].transform.position,
                    VARIABLE.Value, _colorScriptableObjectsService.GetColor(VARIABLE.Value));
            }
            
            foreach (var VARIABLE in _busTypes)
            {
                AddHumanOrBus(_busCellObjects[VARIABLE.Key].transform.position,
                    VARIABLE.Value, _colorScriptableObjectsService.GetColor(VARIABLE.Value));
            }
        }

        private void DestroyExistingObjects()
        {
            foreach (var VARIABLE in _cellObjects)
            {
                Destroy(VARIABLE.Value.gameObject);
            }
            foreach (var VARIABLE in _busCellObjects)
            {
                Destroy(VARIABLE.Value.gameObject);
            }
            foreach (var VARIABLE in _humans)
            {
                Destroy(VARIABLE.Value.gameObject);
            }
            foreach (var VARIABLE in _buses)
            {
                Destroy(VARIABLE.Value.gameObject);
            }
        }

        public bool TryGetLevelSaveObject(out LevelSaveObject levelSaveObject, out string message)
        {
            if (_humans.Count < _minHumanCount) 
            {
                levelSaveObject = null;
                message = "Human count must be at least " + _minHumanCount;
                return false;
            }
            if (_humans.Count % _busSeatCount != 0) 
            {
                levelSaveObject = null;
                message = "Human count must be multiple of " + _busSeatCount;
                return false;
            }
            
            if (_busCellObjects.Count != _buses.Count)
            {
                levelSaveObject = null;
                message = "Bus count must be equal to bus cell count";
                return false;
            }
            
            levelSaveObject = new LevelSaveObject();
            levelSaveObject.Level = -1;
            
            levelSaveObject.CellsIndexes = _cells.Keys.ToList();
            levelSaveObject.CellsValidness = _cells.Values.ToList();
            
            levelSaveObject.HumansIndexes = _humanBusTypes.Keys.ToList();
            levelSaveObject.HumansTypes = _humanBusTypes.Values.ToList();
            
            levelSaveObject.BusesIndexes = _busTypes.Keys.ToList();
            levelSaveObject.BusesTypes = _busTypes.Values.ToList();
            
            levelSaveObject.BoardX = _currX;
            levelSaveObject.BoardY = _currY;
            message = "Level is ready to save";
            return true;
        }
    }
}