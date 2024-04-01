using System.Collections.Generic;
using _Game._Scripts._Human;
using UnityEngine;

namespace _Game._Scripts._Buses
{
    public class BusService
    {
        private Queue<BaseBusController> _createdBuses;

        public void Initialize()
        {
            _createdBuses = new Queue<BaseBusController>();
        }
        
        public void AddBus(BaseBusController bus)
        {
            _createdBuses.Enqueue(bus);
        }
        
        public void RemoveBus()
        {
            _createdBuses.Dequeue();
        }
        
        public bool TryGetBusAtPeek(out BaseBusController bus)
        {
            if (_createdBuses.Count == 0)
            {
                bus = null;
                return false;
            }
            bus = _createdBuses.Peek();
            return true;
        }
        
        public bool TryGetActiveBus(HumanBusType humanBusType, out BaseBusController bus)
        {
            if (_createdBuses.Count == 0)
            {
                bus = null;
                return false;
            }
            bus = _createdBuses.Peek();
            if (!bus.IsArrived()) return false;
            if (humanBusType != bus.GetBusType()) return false;
            return true;
        }
        
        public bool TryAddPassengerToActiveBus(HumanBusType humanBusType, out Vector3 passengerPosition)
        {
            passengerPosition = Vector3.zero;
            if (!TryGetActiveBus(humanBusType, out BaseBusController bus)) return false;
            if (!bus.TryAddPassenger(out Vector3 position)) return false;
            passengerPosition = position;
            return true;
        }
        
        public void SetPassengerParentToBusWhenPassengerArrived(IHumanView passenger)
        {
            _createdBuses.Peek().SetPassengerParentWhenPassengerArrived(passenger);
        }
        
        public void RemovePassengerFromActiveBus(HumanBusType humanBusType)
        {
            if (!TryGetActiveBus(humanBusType, out BaseBusController bus)) return;
            bus.RemovePassenger();
        }

        public Queue<BaseBusController> GetCreatedBuses()
        {
            return _createdBuses;
        }
    }
}