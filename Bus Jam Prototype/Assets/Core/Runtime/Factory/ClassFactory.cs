namespace Core.Factory
{
    public abstract class ClassFactory<TItem> : Factory<TItem, ClassFactoryModel> where TItem : class
    {
    }
}