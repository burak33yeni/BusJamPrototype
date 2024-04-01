namespace Core.Factory
{
    public class ObjectFactoryModel<TObject> : FactoryModel
    {
        internal ObjectFactoryModel() { }
        
        public TObject PrefabObject;
    }
}