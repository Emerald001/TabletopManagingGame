using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectAction : Action {
    public MoveObjectAction(GameObject ObjectToMove, float speed, Transform endPoint) {
        this.ObjectToMove = ObjectToMove;
        this.speed = speed;
        this.endPoint = endPoint;
    }

    private GameObject ObjectToMove;
    private float speed;
    private Transform endPoint;

    public override void OnEnter() {

    }

    public override void OnExit() {

    }

    public override void OnUpdate() {
        if(ObjectToMove.transform.position != endPoint.position) {
            ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, endPoint.position, speed * Time.deltaTime);
        }
        else {
            IsDone = true;
        }
    }
}