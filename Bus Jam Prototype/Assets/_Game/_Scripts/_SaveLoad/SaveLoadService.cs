using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _Game._Scripts._SaveLoad
{
    public static class SaveLoadService
    {
        private static readonly string SaveFolder = "Assets/Resources/" + "LevelSaves/";
        private static readonly string LevelSaveKey = "Level_";
        private static readonly string UnfinishedLevelSaveKey = "UnfinishedLevel";
        private static readonly string LevelFileExtension = ".txt";

        #region Save Load Txt

        public static void SaveLevelToTxt(LevelSaveObject levelSaveObject)
        {
            EnsureSaveFolderExists();
            string levelJson = JsonUtility.ToJson(levelSaveObject);
            File.WriteAllText( SaveFolder + LevelSaveKey + levelSaveObject.Level + 
                               LevelFileExtension, levelJson);
        }
        
        public static void SaveUnfinishedLevelToTxt(UnfinishedLevelSaveObject unfinishedLevelSaveObject)
        {
            EnsureSaveFolderExists();
            string levelJson = JsonUtility.ToJson(unfinishedLevelSaveObject);
            File.WriteAllText( SaveFolder + UnfinishedLevelSaveKey + 
                               LevelFileExtension, levelJson);
        }

        public static bool TryLoadLevelFromTxt(int level, out LevelSaveObject levelSaveObject)
        {
            if (!File.Exists(SaveFolder + LevelSaveKey + level + LevelFileExtension))
            {
                levelSaveObject = null;
                return false;
            }
            string levelJson = File.ReadAllText(SaveFolder + LevelSaveKey + level + 
                                                LevelFileExtension);
            if (string.IsNullOrEmpty(levelJson))
            {
                levelSaveObject = null;
                return false;
            }
            levelSaveObject = JsonUtility.FromJson<LevelSaveObject>(levelJson);
            return true;
        }
        
        public static bool TryLoadUnfinishedLevelFromTxt(out UnfinishedLevelSaveObject unfinishedLevelSaveObject)
        {
            if (!File.Exists(SaveFolder + UnfinishedLevelSaveKey + LevelFileExtension))
            {
                unfinishedLevelSaveObject = null;
                return false;
            }
            string levelJson = File.ReadAllText(SaveFolder + UnfinishedLevelSaveKey + 
                                                LevelFileExtension);
            if (string.IsNullOrEmpty(levelJson))
            {
                unfinishedLevelSaveObject = null;
                return false;
            }
            unfinishedLevelSaveObject = JsonUtility.FromJson<UnfinishedLevelSaveObject>(levelJson);
            return true;
        }
        
        private static void EnsureSaveFolderExists()
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);
        }
        
        #endregion

        #region Save Load Prefs
        
        public static void SaveLevelToPrefs(LevelSaveObject levelSaveObject)
        {
            string levelJson = JsonUtility.ToJson(levelSaveObject);
            PlayerPrefs.SetString(LevelSaveKey + levelSaveObject.Level, levelJson);
        }
        
        public static bool TryLoadLevelFromPrefs(int level, out LevelSaveObject levelSaveObject)
        {
            string levelJson = PlayerPrefs.GetString(LevelSaveKey + level);
            if (string.IsNullOrEmpty(levelJson))
            {
                levelSaveObject = null;
                return false;
            }
            levelSaveObject = JsonUtility.FromJson<LevelSaveObject>(levelJson);
            return true;
        }
        
        #endregion
        
    }
    
    public class LevelSaveObject
    {
        public int Level;
        public List<Vector2Int> CellsIndexes;
        public List<bool> CellsValidness;
        
        public List<Vector2Int> HumansIndexes;
        public List<HumanBusType> HumansTypes;

        public List<int> BusesIndexes;
        public List<HumanBusType> BusesTypes;
        
        public int BoardX, BoardY;
    }
    
    public class UnfinishedLevelSaveObject
    {
        public int Level;
        public List<Vector2Int> CellsIndexes;
        public List<bool> CellsValidness;
        
        public List<Vector2Int> HumansIndexes;
        public List<HumanBusType> HumansTypes;
        
        public int BoardX, BoardY;

        public List<int> BusesIndexes;
        public List<HumanBusType> BusesTypes;
        
        public List<int> CreatedBusesIndexes;
        public List<HumanBusType> CreatedBusesTypes;
        
        public List<int> HumansIndexesInBusStop;
        public List<HumanBusType> HumansTypesInBusStop;
        
        public List<Vector3> HumansPositionsInBus;
    }
}