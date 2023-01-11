using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    public Queue<Action> CurrentQueue = new();
    public Action CurrentAction;

    private readonly System.Action OnEmptyQueue;
    
    public ActionManager(System.Action OnEmptyQueue) {
        this.OnEmptyQueue = OnEmptyQueue;
    }

    public void OnUpdate() {
        if (CurrentAction == null) {
            if (CurrentQueue.Count > 0)
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

            if (CurrentAction.IsDone)
                NextAction();
        }
        else {
            CurrentAction = null;
            OnEmptyQueue.Invoke();
        }
    }

    public void Enqueue(Action action) {
        CurrentQueue.Enqueue(action);
    }

    public void Inject(int index, Action action) {
        List<Action> tmp = new(CurrentQueue);

        tmp.Insert(index, action);

        CurrentQueue = new(tmp);
    }

    public void Clear() {
        CurrentQueue.Clear();
    }
}