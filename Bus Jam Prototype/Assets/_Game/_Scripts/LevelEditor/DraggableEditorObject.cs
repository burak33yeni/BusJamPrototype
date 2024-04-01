using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.LevelEditor
{
    public class DraggableEditorObject : BaseEditorObject
    {
        [SerializeField] private Image _image;
        public HumanBusType humanBusType { get; set; }
        
        public void Init(Color color)
        {
            _image.color = color;
        }
        
        private void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}