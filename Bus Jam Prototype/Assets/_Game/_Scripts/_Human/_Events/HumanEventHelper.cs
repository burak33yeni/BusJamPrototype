using UnityEngine;
using Event = Core.ServiceLocator.Event;

namespace _Game._Scripts._Human._Events
{
    public class HumanEventHelper
    {
    }
    
    public class BoardIndexEmptyEvent : Event
    {
        public Vector2Int index;
        
        public BoardIndexEmptyEvent(Vector2Int index)
        {
            this.index = index;
        }
    }

    public class HumanArrivedBusStopEvent : Event
    {
    }
}