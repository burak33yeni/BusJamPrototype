using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.LevelEditor
{
    public class BusForEditor : BaseEditorObject
    {
        [SerializeField] private Image _image;
        public HumanBusType HumanBusType { get; set; }
        
        public void Init(Color color)
        {
            _image.color = color;
        }
    }
}