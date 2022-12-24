using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : Action {
    public WaitAction(float waitTime) {
        this.waitTime = waitTime;
    }

    private float waitTime;

    public override void OnEnter() { }
    public override void OnExit() { }

    public override void OnUpdate() {
        Timer(ref waitTime);

        if (waitTime <= 0) {
            IsDone = true;
        }
    }

    private float Timer(ref float timer) {
        return timer -= Time.deltaTime;
    }
}