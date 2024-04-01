using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GameConstants
{
    public class CommonUseColoringService : MonoBehaviour, IInitializable
    {
        [SerializeField] private Renderer _groundRenderer;
        [SerializeField] private Renderer _busStopRenderer;
        [SerializeField] private Renderer _cellsOutsideAreaRenderer;
        [SerializeField] private Renderer _roadRenderer;
        
        [Resolve] private ColorScriptableObjectsService _colorScriptableObjectsService;
        public void Initialize()
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_BaseColor", 
                _colorScriptableObjectsService.GetColor(CommonColor.Ground));
            _groundRenderer.SetPropertyBlock(materialPropertyBlock);

            materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_BaseColor", 
                _colorScriptableObjectsService.GetColor(CommonColor.BusStop));
            _busStopRenderer.SetPropertyBlock(materialPropertyBlock);
            
            materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_BaseColor", 
                _colorScriptableObjectsService.GetColor(CommonColor.BoardArea));
            _cellsOutsideAreaRenderer.SetPropertyBlock(materialPropertyBlock);
            
            materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_BaseColor", 
                _colorScriptableObjectsService.GetColor(CommonColor.Road));
            _roadRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}