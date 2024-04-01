using UnityEngine;

namespace _Game._Scripts._ScriptableObjects
{
    [CreateAssetMenu(fileName = "PrefabSO", menuName = "SO/New PrefabSO", order = 0)]
    public class PrefabScriptableObject : ScriptableObject
    {
        public PrefabType prefabType;
        public GameObject prefab; 
    }
}