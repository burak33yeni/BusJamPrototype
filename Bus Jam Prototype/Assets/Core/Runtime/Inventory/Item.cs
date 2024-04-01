namespace Core.Inventory
{
    public abstract class Item
    {
        internal Item()
        {
        }

        internal int amount;
        internal string finalKey;

        protected internal abstract string key { get; }
        protected internal virtual int maxAmount => int.MaxValue;
        protected internal virtual int minAmount => 0;
    }
}