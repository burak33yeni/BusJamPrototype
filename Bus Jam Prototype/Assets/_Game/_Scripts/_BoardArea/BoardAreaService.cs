using UnityEngine;

namespace _Game._Scripts._BoardArea
{
    public class BoardAreaService : MonoBehaviour
    {
        [SerializeField] private Transform _cellsParent;
        [SerializeField] private Transform _busStopCellsParent;
        [SerializeField] private Transform _humanParent;
        [SerializeField] private Transform _busParent;
        public Transform CellsParent => _cellsParent;
        public Transform BusStopCellsParent => _busStopCellsParent;
        public Transform HumanParent => _humanParent;
        public Transform BusParent => _busParent;
    }
}