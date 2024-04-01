using _Game._Scripts._GameConstants;
using Core.Pool;
using TMPro;
using UnityEngine;

namespace _Game._Scripts._Cells
{
    public sealed class BusStopCell : BaseCell
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _canvasRectTransform;
        [SerializeField] private TextMeshProUGUI _text;
        private bool _isAvailable;
        
        public void SetColorAndText(Color color, string text)
        {
            _canvas.worldCamera = Camera.main;
            GameConstantsAndPositioningService.SetBusStopCanvasSpecs(_canvasRectTransform);
            SetColor(color);
            _text.SetText(text);
        }
        
        public void SetAvailability(bool isAvailable)
        {
            _isAvailable = isAvailable;
        }
        
        public bool GetAvailability()
        {
            return _isAvailable;
        }
    }
    
    public class BusStopCellPool : ObjectPool<BusStopCell>
    {
    }
}