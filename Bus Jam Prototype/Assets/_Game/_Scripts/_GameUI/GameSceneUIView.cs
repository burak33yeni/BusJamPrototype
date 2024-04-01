using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts._GameUI
{
    public class GameSceneUIView : MonoBehaviour, IGameSceneUIView
    {
        [SerializeField] private Button goToMenuButton;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Transform popupParent;

        private GameSceneUIModel _model;
        public void Initialize(GameSceneUIModel model)
        {
            _model = model;
            goToMenuButton.onClick.AddListener(() =>
            {
                _model.onGoToMenuButtonClicked?.Invoke();
            });
            
            levelText.SetText(_model.levelText);
            healthText.SetText(_model.healthText);
        }
        
        public Transform GetPopupParent()
        {
            return popupParent;
        }
    }
    
    public interface IGameSceneUIView
    {
        void Initialize(GameSceneUIModel model);
        Transform GetPopupParent();
    }
    
    public class GameSceneUIModel
    {
        public Action onGoToMenuButtonClicked;
        public string levelText;
        public string healthText;
    }
}