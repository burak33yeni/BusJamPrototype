using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts._Popups
{
    public class GameFailPopupView : BasePopupView, IGameFailPopupView
    {
        [SerializeField] private Button goMenuButton;
        [SerializeField] private Button reloadLevelButton;
        [SerializeField] private Button add30secButton;
        [SerializeField] private Button remove30secButton;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        public override void Initialize(GameFailPopupModel model)
        {
            base.Initialize(model);
            goMenuButton.onClick.AddListener(() =>
            {
                model.onGoToMenuButtonClicked?.Invoke();
            });
            reloadLevelButton.onClick.AddListener(() =>
            {
                model.onReloadLevelButtonClicked?.Invoke();
            });
            add30secButton.onClick.AddListener(() =>
            {
                model.onAdd30secButtonClicked?.Invoke();
            });
            remove30secButton.onClick.AddListener(() =>
            {
                model.onRemove30secButtonClicked?.Invoke();
            });
            levelText.SetText(model.levelText);
            healthText.SetText(model.healthText);
            timeText.SetText(model.timeText);
        }
        
        public void UpdateTimeText(string timeText)
        {
            this.timeText.SetText(timeText);
        }
        
        public void SetHealthText(string healthText)
        {
            this.healthText.SetText(healthText);
        }
        public void OpenCloseReloadGameButtons(bool open)
        {
            reloadLevelButton.gameObject.SetActive(open);
            add30secButton.gameObject.SetActive(!open);
            remove30secButton.gameObject.SetActive(!open);
            healthText.gameObject.SetActive(open);
            timeText.gameObject.SetActive(!open);
        }
    }
    
    public class GameFailPopupModel
    {
        public Action onGoToMenuButtonClicked;
        public Action onReloadLevelButtonClicked;
        public Action onAdd30secButtonClicked;
        public Action onRemove30secButtonClicked;
        public string levelText;
        public string healthText;
        public string timeText;
    }
    
    public interface IGameFailPopupView
    {
        void Initialize(GameFailPopupModel model);
        void UpdateTimeText(string timeText);
        void OpenCloseReloadGameButtons(bool open);
        void SetHealthText(string healthText);
    }
}