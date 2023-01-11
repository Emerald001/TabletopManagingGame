using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectAction : Action {
    public MoveObjectAction(GameObject ObjectToMove, float speed, Transform endPoint, string AudioName, float ScreenShakeAmount) {
        this.ObjectToMove = ObjectToMove;
        this.speed = speed;
        this.endPoint = endPoint;
        this.AudioName = AudioName;
        this.ScreenShakeAmount = ScreenShakeAmount;
    }

    public MoveObjectAction(GameObject ObjectToMove, Quaternion ToRotation, float speed, Transform endPoint, string AudioName, float ScreenShakeAmount) {
        this.ObjectToMove = ObjectToMove;
        this.speed = speed;
        this.endPoint = endPoint;
        this.ToRotation = ToRotation;
        this.AudioName = AudioName;
        this.ScreenShakeAmount = ScreenShakeAmount;
    }

    private GameObject ObjectToMove;
    private float speed;
    private Transform endPoint;
    private Quaternion ToRotation;
    private string AudioName;
    private float ScreenShakeAmount;

    public override void OnEnter() {
        ObjectToMove.SetActive(true);

        GameManager.instance.Amanager.PlayAudio(AudioName);
    }

    public override void OnExit() {
        EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, ScreenShakeAmount);
    }

    public override void OnUpdate() {
        if(ObjectToMove.transform.position != endPoint.position) {
            ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, endPoint.position, speed * Time.deltaTime);

            if(ToRotation != null) {
                ObjectToMove.transform.rotation = Quaternion.Lerp(ObjectToMove.transform.rotation, ToRotation, speed * 2 * Time.deltaTime);
            }
        }
        else {
            IsDone = true;
        }
    }
}