using _Game._Scripts._GameScene;
using _Game._Scripts._GeneralGame;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts._GameUI
{
    public class GameSceneUIService : IInitializable
    {
        [Resolve] private Level _level;
        [Resolve] private HealthInventory _healthInventory;
        
        private IGameSceneUIView _view;
        
        public GameSceneUIService(IGameSceneUIView view)
        {
            _view = view;
        }
        
        public void Initialize()
        {
            _view.Initialize(new GameSceneUIModel
            {
                onGoToMenuButtonClicked = OnGoToMenuButtonClicked,
                levelText = "Level " +_level.GetLevelName(),
                healthText = _healthInventory.Get<HealthItem>() + " Health"
            });
        }
        
        private void OnGoToMenuButtonClicked()
        {
            SceneController.LoadStart();
        }
        
        public Transform GetPopupParent()
        {
            return _view.GetPopupParent();
        }
    }
}