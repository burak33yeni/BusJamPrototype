using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game._Scripts.LevelEditor
{
    public abstract class HumanButtonForEditor : MonoBehaviour, IPointerDownHandler
    {
        [Resolve] private LevelEditor _levelEditor;
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        
        [SerializeField] private DraggableEditorObject _draggableEditorObject;
        [SerializeField] private Image _image;
        protected abstract HumanBusType HumanBusType { get; }
        protected abstract CommonColor CommonColor { get; }
        public void Init(Color color)
        {
            _image.color = color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            LevelEditorHelper.DisableCursor();
            if (_levelEditor.DraggableEditorObject != null)
            {
                Destroy(_levelEditor.DraggableEditorObject.gameObject);
            }
            DraggableEditorObject draggable = 
                Instantiate(_draggableEditorObject, _levelEditor.CellHolder.transform);
            draggable.humanBusType = HumanBusType;
            draggable.Init(_colorScriptableObjectsService.GetColor(CommonColor));
            draggable.AddDeleteButtonListener(() =>
            {
                LevelEditorHelper.EnableCursor();
                _levelEditor.AddHumanOrBus(draggable.transform.position, HumanBusType, CommonColor);
                Destroy(draggable.gameObject);
            });
            _levelEditor.DraggableEditorObject = draggable;
        }
    }
}