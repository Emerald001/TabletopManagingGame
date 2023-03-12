using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectAction : Action {
    public MoveObjectAction(GameObject ObjectToMove, float speed, Transform endPoint, string AudioName = "", float ScreenShakeAmount = 0) {
        this.ObjectToMove = ObjectToMove;
        this.speed = speed;
        this.endPoint = endPoint;
        this.AudioName = AudioName;
        this.ScreenShakeAmount = ScreenShakeAmount;
    }

    public MoveObjectAction(GameObject ObjectToMove, float speed, Vector3 endPoint, string AudioName = "", float ScreenShakeAmount = 0) {
        this.ObjectToMove = ObjectToMove;
        this.speed = speed;
        this.Vec3EndPoint = endPoint;
        this.AudioName = AudioName;
        this.ScreenShakeAmount = ScreenShakeAmount;
    }

    private GameObject ObjectToMove;
    private float speed;
    private Transform endPoint;
    private Vector3 Vec3EndPoint;
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
        if (endPoint != null)
            MoveWithTransform();
        else
            MoveWithVector3();
    }

    public void MoveWithTransform() {
        if (ObjectToMove.transform.position != endPoint.position) {
            ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, endPoint.position, speed * Time.deltaTime);
            ObjectToMove.transform.rotation = Quaternion.Lerp(ObjectToMove.transform.rotation, endPoint.rotation, speed * 2 * Time.deltaTime);
        }
        else {
            IsDone = true;
        }
    }

    public void MoveWithVector3() {
        if (ObjectToMove.transform.position != Vec3EndPoint) {
            ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, Vec3EndPoint, speed * Time.deltaTime);
        }
        else {
            IsDone = true;
        }
    }
}