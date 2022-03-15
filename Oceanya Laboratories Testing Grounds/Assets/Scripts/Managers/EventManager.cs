using System.Collections.Generic;
using System;

public static class EventManager 
{
    public enum Events
    {
        Controls_ContextMenu
    }

    public delegate void EventReceiver(params object[] parameters);

    static Dictionary<Events, EventReceiver> _events = new Dictionary<Events, EventReceiver>();
    public static void AddToEvent(Events eventType, EventReceiver method) 
    { 
        if (_events.ContainsKey(eventType)) 
            _events[eventType] += method; 
        else _events.Add(eventType, method); 
    } 
    public static void RemoveFromEvent(Events eventType, EventReceiver method) 
    { 
        if (_events.ContainsKey(eventType)) 
        { 
            _events[eventType] -= method; 
            if (_events[eventType] == null) _events.Remove(eventType);
        } 
    } 
    public static void ClearEvent(Events eventType)
    {
        if (_events.ContainsKey(eventType))
        {
            _events[eventType] = null;
        }
    }
    public static EventReceiver ReturnEvent(Events eventType)
    {
        return _events.ContainsKey(eventType) ? _events[eventType] : null;
    }
    public static void TriggerEvent(Events eventType, params object[] parameters) 
    { 
        if (_events.ContainsKey(eventType)) 
            _events[eventType](parameters); 
    } 
    public static void ClearEvents() 
    { 
        _events.Clear(); 
    } 
}
