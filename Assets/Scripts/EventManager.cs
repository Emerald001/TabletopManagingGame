using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType {
    ON_ENCOUNTER_STARTED,
    ON_CARAVAN_STOPPED,
    ON_ENCOUNTER_ENDED,
    DO_SCREENSHAKE,
    ON_GAME_STARTED,
    ON_GAME_PAUSED,
    ON_GAME_UNPAUSED
}

public static class EventManager {
    private static readonly Dictionary<EventType, System.Action> eventActions = new();

    public static void Subscribe(EventType eventType, System.Action eventToSubscribe) {
        if (!eventActions.ContainsKey(eventType)) {
            eventActions.Add(eventType, null);
        }
        eventActions[eventType] += eventToSubscribe;
    }

    public static void Unsubscribe(EventType eventType, System.Action eventToUnsubscribe) {
        if (eventActions.ContainsKey(eventType)) {
            eventActions[eventType] -= eventToUnsubscribe;
        }
    }

    public static void Invoke(EventType eventType) {
        eventActions[eventType]?.Invoke();
    }
}

public static class EventManager<T> {
    private static readonly Dictionary<EventType, System.Action<T>> eventActions = new();

    public static void Subscribe(EventType eventType, System.Action<T> eventToSubscribe) {
        if (!eventActions.ContainsKey(eventType)) {
            eventActions.Add(eventType, null);
        }
        eventActions[eventType] += eventToSubscribe;
    }

    public static void Unsubscribe(EventType eventType, System.Action<T> eventToUnsubscribe) {
        if (eventActions.ContainsKey(eventType)) {
            eventActions[eventType] -= eventToUnsubscribe;
        }
    }

    public static void Invoke(EventType eventType, T obj) {
        eventActions[eventType]?.Invoke(obj);
    }
}