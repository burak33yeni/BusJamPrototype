using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts._Start
{
    public class StartScreenView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button goLevelButton;
        [SerializeField] private Image goLevelButtonImage;
        [SerializeField] private TextMeshProUGUI levelButtonText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button add30secButton;
        [SerializeField] private Button remove30secButton;
        
        public void Init(StartSceneModel model)
        {
            add30secButton.gameObject.SetActive(model.isAdd30secButtonActive);
            remove30secButton.gameObject.SetActive(model.isRemove30secButtonActive);
            levelButtonText.SetText(model.goLevelButtonText);
            levelText.SetText(model.level);
            goLevelButton.onClick.AddListener(() =>
            {
                model.goLevelButtonAction?.Invoke();
            });
            add30secButton.onClick.AddListener(() =>
            {
                model.add30secButtonAction?.Invoke();
            });
            remove30secButton.onClick.AddListener(() =>
            {
                model.remove30secButtonAction?.Invoke();
            });
        }
        
        public void SetTimerText(string text)
        {
            timerText.SetText(text);
        }

        public void SetGoLevelButtonActivenessAsColor(bool isActive)
        {
            goLevelButtonImage.color = isActive ? Color.green : Color.gray;
        }
        
        public void SetAddRemoveTimeButtonsActiveness(bool isAddActive, bool isRemoveActive)
        {
            add30secButton.gameObject.SetActive(isAddActive);
            remove30secButton.gameObject.SetActive(isRemoveActive);
        }
    }
    
    public class StartSceneModel
    {
        public string level;
        public string goLevelButtonText;
        public Action goLevelButtonAction;
        public Action add30secButtonAction;
        public Action remove30secButtonAction;
        public bool isAdd30secButtonActive;
        public bool isRemove30secButtonActive;
    }
}