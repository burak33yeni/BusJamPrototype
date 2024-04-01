using System.Collections.Generic;
using _Game._Scripts._Cells._Factory;
using _Game._Scripts._GameScene;
using _Game._Scripts._GeneralGame;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GameConstants
{
    public class GameConstantsAndPositioningService
    {
        [Resolve] private Level _level;
        [Resolve] private CellCreator _cellCreator;

        #region Board Cells
        public static readonly int _maxBoardCellCountOnAxis = 8;
        private static readonly float _topCenterZ = -18.5f;
        private static readonly float _cellPosDifference = 11f;
        private static readonly float _evenCellCountXDifference = 5.5f;
        private static readonly float _yPos = 0.21f;
        private static readonly float _defaultCellScale = 1f;

        #endregion

        #region Bus Stop Cells

        private static readonly int _maxBusStopCellCount = 7;
        private static readonly float _busStopCellZ = -1f;
        private static readonly float _busStopCellPosDifference = 11f;
        private static readonly int _firstBusStopCellUnlockedLevel = 3;
        private static readonly int _lastBusStopCellUnlockedLevel = 5;
        public static int FirstBusStopCellUnlockedLevel => _firstBusStopCellUnlockedLevel;
        public static int LastBusStopCellUnlockedLevel => _lastBusStopCellUnlockedLevel;

        #endregion

        #region Bus

        private static readonly float _newCreatedBusXPos = -100f;
        private static readonly float _queuedBusXPos = -40f;
        private static readonly float _filledBusMaxXPos = 100f;
        
        private static readonly int _standardBusMaxPassengers = 3;
        public static readonly int StandardBusMaxPassengers = _standardBusMaxPassengers;

        #endregion

        #region Other

        private static readonly int _maxHealth = 4;
        public static int MaxHealth => _maxHealth;
        private static readonly string _timerFormat = "mm\\:ss";
        public static string TimerFormat => _timerFormat;

        #endregion
        
        private Vector3 _bottomLeftCellPos;
        public void SetXRowCountIsOddOrEven(bool isEven)
        {
            _bottomLeftCellPos = new Vector3(
                -1 * (_level.GetBoardXAndY().x / 2) * _cellPosDifference + 
                (isEven ? _evenCellCountXDifference : 0),
                _yPos,
                _topCenterZ - _cellPosDifference * (_level.GetBoardXAndY().y - 1));
        }
        public Vector3 GetWaitingCellPosition(Vector2Int waitingCellIndex)
        {
            return new Vector3(
                _bottomLeftCellPos.x + waitingCellIndex.x * _cellPosDifference,
                _bottomLeftCellPos.y,
                _bottomLeftCellPos.z + waitingCellIndex.y * _cellPosDifference);
        }
        
        public Vector3 GetBusStopCellPosition(int busStopCellX)
        {
            return new Vector3(
                -1 * (_maxBusStopCellCount / 2 - busStopCellX) * _busStopCellPosDifference
                + (_maxBusStopCellCount % 2 == 0 ? _busStopCellPosDifference / 2f : 0),
                _yPos,
                _busStopCellZ);
        }
        
        public Vector3 GetHumanPosition(Vector2Int humanIndex)
        {
            return GetWaitingCellPosition(humanIndex);
        }
        
        public static Vector3 GetCellScale()
        {
            return Vector3.one * _defaultCellScale;
        }
        
        public static int GetMaxBusStopCellCount()
        {
            return _maxBusStopCellCount;
        }

        public static void SetBusStopCanvasSpecs(RectTransform busStopCanvas)
        {
            busStopCanvas.localScale = Vector3.one * 0.03f;
            busStopCanvas.localRotation = Quaternion.Euler(90, 0, 0);
            busStopCanvas.localPosition = new Vector2(0, 0.1f);
            busStopCanvas.sizeDelta = Vector2.one * 330f;
        }
        
        public bool TryGetHumanPathToBusStopCell(Stack<Vector2Int> boardCellIndexes, 
            int busStopCellIndex, out Stack<Vector3> path)
        {
            path = new Stack<Vector3>();
            if (_cellCreator.TryGetBusStopCellPosition(busStopCellIndex, out Vector3 busStopPos))
                path.Push(busStopPos);
            else return false;
            return TryAddBoardCellsPositionsToStack(boardCellIndexes, path);
        }

        public bool TryGetHumanPathDirectlyToBus(Stack<Vector2Int> boardCellIndexes, 
            Vector3 busPassengerPosition, out Stack<Vector3> path)
        {
            path = new Stack<Vector3>();
            path.Push(busPassengerPosition);
            return TryAddBoardCellsPositionsToStack(boardCellIndexes, path);
        }
        
        private bool TryAddBoardCellsPositionsToStack(Stack<Vector2Int> boardCellIndexes, Stack<Vector3> path)
        {
            foreach (Vector2Int index in boardCellIndexes)
            {
                if (_cellCreator.TryGetBoardCellPosition(index, out Vector3 pos))
                    path.Push(pos);
                else return false;
            }
            return true;
        }
        
        public float GetNewBusXPos()
        {
            return _newCreatedBusXPos;
        }
        
        public float GetQueuedBusXPos()
        {
            return _queuedBusXPos;
        }
        
        public float GetFilledBusMaxXPos()
        {
            return _filledBusMaxXPos;
        }

    }
}