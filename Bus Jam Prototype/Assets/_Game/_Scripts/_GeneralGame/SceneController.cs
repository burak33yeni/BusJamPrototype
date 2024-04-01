using _Game._Scripts._GameScene;
using UnityEngine.SceneManagement;

namespace _Game._Scripts._GeneralGame
{
    public static class SceneController
    {
        private static readonly string START = "Start";
        private static readonly string GAME = "Game";
        private static readonly string EDITOR = "Editor";
        private static readonly string END = "End";
    
        public static void LoadStart()
        {
#if UNITY_EDITOR
            if (GameInitializer.TestMode)
            {
                GameInitializer.TestMode = false;
                SceneManager.LoadScene(EDITOR);
            }
            else
                SceneManager.LoadScene(START);
#else
            SceneManager.LoadScene(START);
#endif            
        }
    
        public static void LoadGame()
        {
#if UNITY_EDITOR
            if (GameInitializer.TestMode)
            {
                GameInitializer.TestMode = false;
                SceneManager.LoadScene(EDITOR);
            }
            else
                SceneManager.LoadScene(GAME);
#else
            SceneManager.LoadScene(GAME);
#endif
        }
    
        public static void LoadEnd()
        {
#if UNITY_EDITOR
            if (GameInitializer.TestMode)
            {
                GameInitializer.TestMode = false;
                SceneManager.LoadScene(EDITOR);
            }
            else
                SceneManager.LoadScene(END);
#else
            SceneManager.LoadScene(END);
#endif
        }
        
        public static void LoadEditor()
        {
            SceneManager.LoadScene(EDITOR);
        }

        public static bool IsStartScene()
        {
            return SceneManager.GetActiveScene().name.Equals(START);
        }
    
        public static bool IsGameScene()
        {
            return SceneManager.GetActiveScene().name.Equals(GAME);
        }
    
        public static bool IsEditorScene()
        {
            return SceneManager.GetActiveScene().name.Equals(EDITOR);
        }
    }
}