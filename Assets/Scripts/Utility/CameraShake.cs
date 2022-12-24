using UnityEngine;

public class CameraShake : MonoBehaviour {
    public Transform cameraTransform;
    private Vector3 orignalCameraPos;

    public float shakeDuration = 2f;
    public float shakeAmount = 0.7f;

    private bool canShake = false;
    private float _shakeTimer;

    private void OnEnable() {
        EventManager<float>.Subscribe(EventType.DO_SCREENSHAKE, ShakeCamera);
    }

    private void OnDisable() {
        EventManager<float>.Unsubscribe(EventType.DO_SCREENSHAKE, ShakeCamera);
    }

    void Start() {

    }

    void Update() {
        if (canShake) {
            StartCameraShakeEffect();
        }
    }

    public void ShakeCamera(float intensity) {
        shakeAmount = intensity;
        orignalCameraPos = cameraTransform.localPosition;

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