using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.LevelEditor
{
    public abstract class BaseEditorObject : MonoBehaviour
    {
        [SerializeField] Button _deleteButton;
        
        public void AddDeleteButtonListener(Action action)
        {
            _deleteButton.onClick.AddListener(()=>
            {
                action?.Invoke();
            });
        }
    }
}