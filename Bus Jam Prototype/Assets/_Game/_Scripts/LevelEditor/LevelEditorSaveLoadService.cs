using _Game._Scripts._GeneralGame;
using _Game._Scripts._SaveLoad;
using Core.ServiceLocator;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.LevelEditor
{
    public class LevelEditorSaveLoadService : MonoBehaviour, IInitializable
    {
        public static readonly string QuickTestKey = "QuickTest";
        [Resolve] private LevelEditor _levelEditor;
        #region Save

        [SerializeField] private Button _saveButton;
        [SerializeField] private TextMeshProUGUI _saveInfoText;
        [SerializeField] private TMP_InputField _saveInputField;
        [SerializeField] private Button _overwriteButton;

        #endregion

        #region Load

        [SerializeField] private Button _loadButton;
        [SerializeField] private TextMeshProUGUI _loadInfoText;
        [SerializeField] private TMP_InputField _loadInputField;

        #endregion
        
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _quickTestButton;
        
        public void Initialize()
        {
            _overwriteButton.gameObject.SetActive(false);
            _loadInfoText.SetText("");
            _saveInfoText.SetText("");
            _levelText.SetText("Unloaded");
            _saveButton.onClick.AddListener(SaveLevelButtonClicked);
            _loadButton.onClick.AddListener(LoadLevel);
            _overwriteButton.onClick.AddListener(OverwriteLevel);
            _quickTestButton.onClick.AddListener(QuickTestButton);
        }

        private void QuickTestButton()
        {
            if (!_levelEditor.TryGetLevelSaveObject(out var levelSaveObject, out string message))
            {
                _saveInfoText.SetText(message);
                return;
            }
            levelSaveObject.Level = -1;
            // Save level
            SaveLoadService.SaveLevelToTxt(levelSaveObject);
            AssetDatabase.Refresh();
            PlayerPrefs.SetInt(QuickTestKey, 1);
            SceneController.LoadGame();
        }

        #region Load

        private void LoadLevel()
        {
            if (!CheckLevelTextIsValid(out int level)) return;
            
            if (!SaveLoadService.TryLoadLevelFromTxt(level, out LevelSaveObject levelSaveObject))
            {
                _loadInfoText.SetText("Level " + level + " does not exist!");
                return;
            }
            
            _levelEditor.LoadLevel(levelSaveObject);
            _loadInfoText.SetText("Level " + level + " loaded!");
            _levelText.SetText(levelSaveObject.Level.ToString());
        }

        private bool CheckLevelTextIsValid(out int level)
        {
            level = -1;
            if (string.IsNullOrEmpty(_loadInputField.text))
            {
                _loadInfoText.SetText("Please enter level!");
                return false;
            }
            if (!int.TryParse(_loadInputField.text, out level))
            {
                _loadInfoText.SetText("Please enter number!");
                return false;
            }

            return true;
        }

        #endregion
        #region Save And Overwrite
        
        private void OverwriteLevel()
        {
            if (!CheckLevelTextAndSaveObjectIsValid(out var level, out var levelSaveObject)) return;
            levelSaveObject.Level = level;
            // Overwrite level
            SaveLoadService.SaveLevelToTxt(levelSaveObject);
            AssetDatabase.Refresh();
            _saveInfoText.SetText("Level overwrites " + level + " !");
            _overwriteButton.gameObject.SetActive(false);
        }

        private void SaveLevelButtonClicked()
        {
            if (!CheckLevelTextAndSaveObjectIsValid(out var level, out var levelSaveObject)) return;
            levelSaveObject.Level = level;
            if (SaveLoadService.TryLoadLevelFromTxt(level, out LevelSaveObject existingLvl))
            {
                _overwriteButton.gameObject.SetActive(true);
                _saveInfoText.SetText("Level " +
                                      existingLvl.Level + 
                                      " already exists! Overwrite?");
                return;
            }
            
            // Save level
            SaveLoadService.SaveLevelToTxt(levelSaveObject);
            AssetDatabase.Refresh();
            _saveInfoText.SetText("Level saved as " + level);
        }
        
        private bool CheckLevelTextAndSaveObjectIsValid(out int level, out LevelSaveObject levelSaveObject)
        {
            level = -1;
            levelSaveObject = null;
            if (string.IsNullOrEmpty(_saveInputField.text))
            {
                _saveInfoText.SetText("Please enter level!");
                return false;
            }
            if (!int.TryParse(_saveInputField.text, out level))
            {
                _saveInfoText.SetText("Please enter number!");
                return false;
            }
            if (!_levelEditor.TryGetLevelSaveObject(
                    out levelSaveObject, out string message))
            {
                _saveInfoText.SetText(message);
                return false;
            }

            return true;
        }
        
        #endregion
       
    }
}