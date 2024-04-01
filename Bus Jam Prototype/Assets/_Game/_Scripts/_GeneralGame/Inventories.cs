using Core.Inventory;

namespace _Game._Scripts._GeneralGame
{
    #region Level

    public class LevelInventory : PersistentInventory
    {
        protected override string Key => "LevelInventory";
    }
    public class LevelItem : ConsumableItem
    {
        protected internal override string key => "LevelNumber";
        protected internal override int minAmount => 1;
    }
    
    public class LastPlayedLevelItem : ConsumableItem
    {
        protected internal override string key => "LastCompletedLevelNumber";
        protected internal override int minAmount => 0;
    }

    #endregion

    #region Health

    public class HealthInventory : PersistentInventory
    {
        protected override string Key => "HealthInventory";
    }
    public class HealthItem : ConsumableItem
    {
        protected internal override string key => "CurrentHealth";
        protected internal override int minAmount => 0;
    }
    
    public class FirstTimeCheck : ConsumableItem
    {
        protected internal override string key => "FirstTimeCheck";
        protected internal override int minAmount => 0;
    }

    #endregion
}