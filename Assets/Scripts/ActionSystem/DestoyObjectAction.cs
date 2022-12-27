using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyObjectAction : Action {
    public DestoyObjectAction(GameObject gameObject) {
        this.gameObject = gameObject;
    }

    GameObject gameObject;

    public override void OnEnter() {
        MonoBehaviour.Destroy(gameObject);

        IsDone = true;
    }

    public override void OnExit() {

    }

    public override void OnUpdate() {

    }
}