using UnityEngine;

namespace _Game._Scripts._Popups
{
    public class BasePopupView : MonoBehaviour
    {
        [SerializeField] private RectTransform _popupRectTransform;
        public virtual void Initialize(GameFailPopupModel model)
        {
            _popupRectTransform.anchoredPosition = Vector2.zero;
            _popupRectTransform.sizeDelta = Vector2.zero;
            _popupRectTransform.localScale = Vector3.one;
        }
    }
}