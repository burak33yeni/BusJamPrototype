using System;
using System.Collections.Generic;
using Core.Tools;

namespace Core.ServiceLocator
{
    internal class EventBus : Bus<Event>
    {
        private static List<Action> _SentEventList = new();
        private static bool _IsInvoking;
        private static int _InvokeCounter;
        private static int _PauseCounter;
        
        internal void TrySend<TEvent>(TEvent payload) where TEvent : Event
        {
            if (_IsInvoking || _PauseCounter > 0)
            {
                _SentEventList.Insert(_InvokeCounter,() => Send(payload));
                _InvokeCounter++;
                return;
            }

            _IsInvoking = true;
            Send(payload);
            InvokeQueuedEvents();
            _IsInvoking = false;
        }

        private void Send<TEvent>(TEvent payload) where TEvent : Event
        {
            Map<long, object> actionsDict = GetTypeMap(typeof(TEvent));
            SortedList<long, object> sortedActions = new();

            foreach (KeyValuePair<long, object> actionPair in actionsDict)
            {
                sortedActions.Add(actionPair.Key, actionPair.Value);
            }

            foreach (KeyValuePair<long, object> sortedActionPair in sortedActions)
            {
                if (sortedActionPair.Value is Action<TEvent> typedAction)
                {
                    if (!actionsDict.ContainsValue(typedAction)) continue;
                    typedAction?.Invoke(payload);
                }
                else
                {
                    Action typelessAction = (Action) sortedActionPair.Value;
                    if (!actionsDict.ContainsValue(typelessAction)) continue;
                    typelessAction?.Invoke();
                }
            }
        }

        internal static void PauseSending()
        {
            _PauseCounter++;
        }
        
        internal static void UnpauseSending()
        {
            _PauseCounter--;
            if(_IsInvoking) return;
            InvokeQueuedEvents();
        }

        private static void InvokeQueuedEvents()
        {
            _InvokeCounter = 0;
            while (_SentEventList.Count > 0 && _PauseCounter == 0)
            {
                Action sentEvent = _SentEventList[0];
                _SentEventList.RemoveAt(0);
                sentEvent();
                _InvokeCounter = 0;
            }
        }
    }
}