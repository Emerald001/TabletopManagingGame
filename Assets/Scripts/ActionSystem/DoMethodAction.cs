using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoMethodAction : Action {
    public DoMethodAction(System.Action action) {
        this.action = action;
    }

    private System.Action action;

    public override void OnEnter() {
        action.Invoke();

        IsDone = true;
    }

    public override void OnExit() {
    }

    public override void OnUpdate() {
    }
}

public class DoMethodAction<T> : Action
{
    public DoMethodAction(System.Action<T> action, T arg) {
        this.action = action;
        this.arg = arg;
    }

    private System.Action<T> action;
    private T arg;

    public override void OnEnter() {
        action.Invoke(arg);

        IsDone = true;
    }

    public override void OnExit() {
    }

    public override void OnUpdate() {
    }
}