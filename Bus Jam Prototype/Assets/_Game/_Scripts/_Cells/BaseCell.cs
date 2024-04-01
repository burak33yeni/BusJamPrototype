using _Game._Scripts._GameConstants;
using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._Cells
{
    public abstract class BaseCell : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private MeshRenderer _renderer;
        
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        public void SetPositionAndScale(Vector3 position)
        {
            _transform.localPosition = position;
            _transform.localScale = GameConstantsAndPositioningService.GetCellScale();
        }

        public void SetColor(Color color)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_BaseColor", color);
            _renderer.SetPropertyBlock(materialPropertyBlock);
        }
        public Vector3 GetWorldPosition()
        {
            return _transform.position;
        }
    }
    
}