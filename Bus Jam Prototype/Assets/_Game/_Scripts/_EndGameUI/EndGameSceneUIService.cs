using System;
using _Game._Scripts._GeneralGame;
using _Game._Scripts._SaveLoad;
using Core.ServiceLocator;

namespace _Game._Scripts._EndGameUI
{
    public class EndGameSceneUIService : IInitializable
    {
        [Resolve] private HealthInventory _healthInventory;
        [Resolve] private LevelInventory _levelInventory;
        
        private IEndGameSceneUIView _view;
        private TimeSpan _remainingGameplayTime;
        
        public EndGameSceneUIService(IEndGameSceneUIView view)
        {
            _view = view;
        }
        
        public void Initialize()
        {
            int health = _healthInventory.Get<HealthItem>();
            int level = _levelInventory.Get<LastPlayedLevelItem>();
            int nextLevel = _levelInventory.Get<LevelItem>();
            bool nextLevelAvailable = 
                SaveLoadService.TryLoadLevelFromTxt(nextLevel, out LevelSaveObject freshLevel);
            _view.Initialize(new EndGameSceneUIModel
            {
                onGoToMenuButtonClicked = OnGoToMenuButtonClicked,
                levelText = "Level " + level,
                healthText = "Health: " + health,
                goToLevelButtonText = GetLevelButtonText(nextLevelAvailable, nextLevel),
                isGoToLevelButtonActive = nextLevelAvailable,
                onGoToLevelButtonClicked = OnGoToLevelButtonClicked
            });
        }

        private void OnGoToLevelButtonClicked()
        {
            SceneController.LoadGame();
        }

        private string GetLevelButtonText(bool nextLevelAvailable, int nextLevel)
        {
            return nextLevelAvailable ? "Go to level " + nextLevel : "All levels are completed!";
        }

        private void OnGoToMenuButtonClicked()
        {
            SceneController.LoadStart();
        }
    }
}