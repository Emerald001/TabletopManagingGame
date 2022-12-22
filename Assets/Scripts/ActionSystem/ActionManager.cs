using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    public Queue<Action> CurrentQueue = new();
    public Action CurrentAction;

    public void OnUpdate() {
        if (CurrentAction == null)
            return;

        CurrentAction.OnUpdate();

        if (CurrentAction.IsDone)
            NextAction();
    }

    private void NextAction() {
        if (CurrentAction != null)
            CurrentAction.OnExit();

        if(CurrentQueue.Count > 0) {
            CurrentAction = CurrentQueue.Dequeue();
            CurrentAction.OnEnter();
        }
        else {
            CurrentAction = null;
            Debug.Log("Last Action in Queue");
        }
    }

    private void Enqueue(Action action) {
        CurrentQueue.Enqueue(action);
    }
}