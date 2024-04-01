using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts._EndGameUI
{
    public class EndGameSceneUIView : MonoBehaviour, IEndGameSceneUIView
    {
        [SerializeField] private Button goToMenuButton;
        [SerializeField] private Button goLevelButton;
        [SerializeField] private Image goLevelButtonImage;
        [SerializeField] private TextMeshProUGUI goLevelButtonText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI healthText;
        public void Initialize(EndGameSceneUIModel model)
        {
            goToMenuButton.onClick.AddListener(() =>
            {
                model.onGoToMenuButtonClicked?.Invoke();
            });
            levelText.SetText(model.levelText);
            healthText.SetText(model.healthText);
            if (!model.isGoToLevelButtonActive)
                goLevelButtonImage.color = Color.gray;
            else
            {
                goLevelButton.onClick.AddListener(() =>
                {
                    model.onGoToLevelButtonClicked?.Invoke();
                });
            }
            goLevelButtonText.SetText(model.goToLevelButtonText);
        }
    }
    
    public interface IEndGameSceneUIView
    {
        void Initialize(EndGameSceneUIModel model);
    }
    
    public class EndGameSceneUIModel
    {
        public Action onGoToMenuButtonClicked;
        public Action onGoToLevelButtonClicked;
        public bool isGoToLevelButtonActive;
        public string goToLevelButtonText;
        public string levelText;
        public string healthText;
    }
}