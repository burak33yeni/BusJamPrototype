using Core.Factory;
using UnityEngine;

namespace _Game._Scripts._Popups._PopupFactory
{
    public class GameFailPopupFactory : ObjectFactory<GameFailPopupView>
    {
        public IGameFailPopupView Spawn(Transform model)
        {
            IGameFailPopupView obj = base.Spawn(model);
            return obj;
        }

        public void Despawn<TType>(IGameFailPopupView item) where TType : Component
        {
            Object.Destroy((item as TType).gameObject);
        }
    }
}