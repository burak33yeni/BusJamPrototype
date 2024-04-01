using System.Collections.Generic;
using _Game._Scripts._GameScene;
using _Game._Scripts._GeneralGame;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Human
{
    public class HumanControllerHelper
    {
        [Resolve] private Level _level;
        
        public bool CanMoveToBusStopCells(Vector2Int humanIndex, out Stack<Vector2Int> validIndexes)
        {
            List<Vector2Int> checkedNonValidCells = new List<Vector2Int>();
            validIndexes = new Stack<Vector2Int>();
            return CheckCells(humanIndex, ref checkedNonValidCells, ref validIndexes, ref humanIndex);
        }
        
        private bool CheckCells(Vector2Int index, 
            ref List<Vector2Int> checkedNonValidCells, 
            ref Stack<Vector2Int> validIndexes, ref Vector2Int startIndex)
        {
            if (validIndexes.Contains(index))
            {
                return false;
            }
            if (index != startIndex)
            {
                if (checkedNonValidCells.Contains(index)) return false;
                if (!_level.IsCellAvailableToMove(index))
                {
                    if (!checkedNonValidCells.Contains(index))
                        checkedNonValidCells.Add(index);
                    return false;
                }
                validIndexes.Push(index);
            }

            if (_level.IsThisTheCellToMoveBusStopCells(index)) return true;

            if (CheckCells(index + Vector2Int.up, ref checkedNonValidCells,
                    ref validIndexes, ref startIndex) ||
                CheckCells(index + Vector2Int.left, ref checkedNonValidCells,
                    ref validIndexes, ref startIndex) ||
                CheckCells(index + Vector2Int.right, ref checkedNonValidCells,
                    ref validIndexes, ref startIndex) ||
                CheckCells(index + Vector2Int.down, ref checkedNonValidCells,
                    ref validIndexes, ref startIndex))
            {
                return true;
            }
            else
            {
                if (validIndexes.Contains(index))
                    checkedNonValidCells.Add(validIndexes.Pop());
                return false;
            }
        }
    }
}