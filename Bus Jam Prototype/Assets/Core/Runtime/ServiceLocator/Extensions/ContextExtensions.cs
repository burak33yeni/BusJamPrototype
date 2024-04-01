using System;
using Core.ServiceLocator;

public static class ContextExtensions
{
    public static TService Resolve<TService>(this object obj)
    {
        return Context.GetContextForObject(obj).Resolve<TService>();
    }

    public static void SendEvent<TEvent>(this object obj, TEvent eventData) where TEvent : Event
    {
        Context.GetContextForObject(obj).SendEvent(eventData);
    }

    public static void FollowEvent<TEvent>(this object obj, Action<TEvent> action, int invocationOrder = 0) where TEvent : Event
    {
        Context.GetContextForObject(obj).FollowEvent(action, invocationOrder);
    }
}
