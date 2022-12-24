using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    public Queue<Action> CurrentQueue = new();
    public Action CurrentAction;

    private System.Action OnEmptyQueue;

    public ActionManager(System.Action OnEmptyQueue) {
        this.OnEmptyQueue = OnEmptyQueue;
    }

    public void OnUpdate() {
        if (CurrentAction == null) {
            NextAction();
            return;
        }

        CurrentAction.OnUpdate();

        if (CurrentAction.IsDone)
            NextAction();
    }

    private void NextAction() {
        if (CurrentAction != null)
            CurrentAction.OnExit();

        if (CurrentQueue.Count > 0) {
            CurrentAction = CurrentQueue.Dequeue();
            CurrentAction.OnEnter();
        }
        else {
            CurrentAction = null;
            OnEmptyQueue.Invoke();
        }
    }

    public void Enqueue(Action action) {
        CurrentQueue.Enqueue(action);
    }
}