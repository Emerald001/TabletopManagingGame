using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCallAction : Action {
    public WaitForCallAction(EventType eventType) {
        this.eventType = eventType;
    }

    private EventType eventType;

    public override void OnEnter() {
        EventManager.Subscribe(eventType, Listening);
    }

    public override void OnExit() {
        EventManager.Unsubscribe(eventType, Listening);
    }

    public override void OnUpdate() {

    }

    public void Listening() {
        IsDone = true;
    }
}