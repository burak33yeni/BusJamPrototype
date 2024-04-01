using Core.Factory;

namespace _Game._Scripts._Human._HumanFactory
{
    public class HumanFactory : ClassFactory<BaseHumanController>
    {
        protected override BaseHumanController Create<TType>()
        {
            BaseHumanController humanController = new TType();
            return humanController;
        }

        public BaseHumanController Spawn<TType>() where TType : BaseHumanController, new()
        {
            return base.Spawn<TType>();
        }
    }
}