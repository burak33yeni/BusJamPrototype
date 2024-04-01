using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.LevelEditor
{
    public class HumanForEditor : BaseEditorObject
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _bgImage;
        public HumanBusType humanBusType { get; set; }
        public void Init(Color color)
        {
            _image.color = color;
        }
    }
    
}