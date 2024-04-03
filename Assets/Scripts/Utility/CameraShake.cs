using UnityEngine;

public class CameraShake : MonoBehaviour {
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private float shakeAmount = 0.7f;

    private Vector3 orignalCameraPos;
    private bool canShake = false;
    private float shakeTimer;

    private void OnEnable() {
        EventManager<CameraEventType, float>.Subscribe(CameraEventType.DO_SCREENSHAKE, ShakeCamera);
    }
    private void OnDisable() {
        EventManager<CameraEventType, float>.Unsubscribe(CameraEventType.DO_SCREENSHAKE, ShakeCamera);
    }

    private void Update() {
        if (canShake)
            StartCameraShakeEffect();
    }

    private void ShakeCamera(float intensity) {
        shakeAmount = intensity;
        orignalCameraPos = cameraTransform.localPosition;

        canShake = true;
        shakeTimer = shakeDuration;
    }

    private void StartCameraShakeEffect() {
        if (shakeTimer > 0) {
            cameraTransform.localPosition = orignalCameraPos + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else {
            shakeTimer = 0f;
            cameraTransform.position = orignalCameraPos;
            canShake = false;
        }
    }
}