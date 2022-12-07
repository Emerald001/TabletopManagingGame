using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookInteract : MonoBehaviour
{
    public Transform StandardPos;
    public Transform LiftedPos;

    private bool hovering;

    void Start(){
        
    }

    void Update() {
        CheckForHover();

        if (hovering) {
            transform.position = Vector3.MoveTowards(transform.position, LiftedPos.position, 5 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, LiftedPos.rotation, 40 * Time.deltaTime);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, StandardPos.position, 5 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, StandardPos.rotation, 40 * Time.deltaTime);
        }
    }

    public void CheckForHover() {
        Ray target = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(target, out var hit, 10000)) {
            if (hit.transform.gameObject == gameObject)
                hovering = true;
            else
                hovering = false;
        }
    }
}