using System;
using System.Collections.Generic;
using _Game._Scripts._ScriptableObjects;
using UnityEngine;

namespace _Game._Scripts._ScriptableObjects
{
    public class PrefabScriptableObjectsService : MonoBehaviour
    {
        [SerializeField] private List<PrefabScriptableObjectConfig> configs;
    
        public TComponent GetPrefab<TComponent>(PrefabType prefabType) where TComponent : Component
        {
            foreach (var config in configs)
            {
                if (config.prefabSo.prefabType != prefabType) continue;
                TComponent component = config.prefabSo.prefab.GetComponent<TComponent>();
                if (component != null)
                    return component;
            }
            throw new Exception(prefabType + " Scriptable Object not found!");
        }
    }
    
}

[Serializable]
public class PrefabScriptableObjectConfig
{
    public PrefabScriptableObject prefabSo;
}