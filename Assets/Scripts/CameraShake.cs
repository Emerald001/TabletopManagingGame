using UnityEngine;

public class CameraShake : MonoBehaviour {
    public Transform cameraTransform;
    private Vector3 orignalCameraPos;

    public float shakeDuration = 2f;
    public float shakeAmount = 0.7f;

    private bool canShake = false;
    private float _shakeTimer;

    void Start() {
        orignalCameraPos = cameraTransform.localPosition;
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            ShakeCamera();
        }

        if (canShake) {
            StartCameraShakeEffect();
        }
    }

    public void ShakeCamera() {
        canShake = true;
        _shakeTimer = shakeDuration;
    }

    public void StartCameraShakeEffect() {
        if (_shakeTimer > 0) {
            cameraTransform.localPosition = orignalCameraPos + Random.insideUnitSphere * shakeAmount;
            _shakeTimer -= Time.deltaTime;
        }
        else {
            _shakeTimer = 0f;
            cameraTransform.position = orignalCameraPos;
            canShake = false;
        }
    }
}