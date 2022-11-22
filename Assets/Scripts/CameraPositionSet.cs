using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionSet : MonoBehaviour
{
    public Transform MainPos;
    public Transform CaravanPos;
    public Transform ResourcePos;
    public Transform CardPos;
    public Vector3 CardRot;

    public float cameraMoveSpeed;

    private Transform CurrentPos;
    private Transform Maincam;

    private void Start() {
        Maincam = Camera.main.transform;

        CurrentPos = MainPos;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            if (CurrentPos == MainPos) {
                CurrentPos = CaravanPos;
            }
            else if(CurrentPos == CaravanPos) {
                CurrentPos = ResourcePos;
            }
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            if (CurrentPos == CaravanPos) {
                CurrentPos = MainPos;
            }
            else if (CurrentPos == ResourcePos) {
                CurrentPos = CaravanPos;
            }
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            if (CurrentPos != CardPos) {
                CurrentPos = CardPos;
            }
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            if (CurrentPos == CardPos) {
                CurrentPos = MainPos;
            }
        }

        Maincam.position = Vector3.MoveTowards(Maincam.position, CurrentPos.position, cameraMoveSpeed * Time.deltaTime);
        Maincam.eulerAngles = Vector3.MoveTowards(Maincam.eulerAngles, CurrentPos.eulerAngles, cameraMoveSpeed * 15 * Time.deltaTime);
    }
}