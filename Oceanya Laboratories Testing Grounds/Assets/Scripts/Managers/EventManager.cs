using System.Collections.Generic;
using System;

public static class EventManager 
{
    public enum Events
    {
        Controls_WaitingForAction
    }

    static Dictionary<Events, Action> _events = new Dictionary<Events, Action>();
    public static void AddToEvent(Events eventType, Action method) 
    { 
        if (_events.ContainsKey(eventType)) 
            _events[eventType] += method; 
        else _events.Add(eventType, method); 
    } 
    public static void RemoveFromEvent(Events eventType, Action method) 
    { 
        if (_events.ContainsKey(eventType)) 
        { 
            _events[eventType] -= method; 
            if (_events[eventType] == null) _events.Remove(eventType);
        } 
    } 
    public static void TriggerEvent(Events eventType) 
    { 
        if (_events.ContainsKey(eventType)) 
            _events[eventType](); 
    } 
    public static void ClearEvents() 
    { 
        _events.Clear(); 
    } 
}
