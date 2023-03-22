using System;
using Unity.VisualScripting;
using UnityEngine;

public class HeadMovementReader : MonoBehaviour {
    public delegate void ShowJosh(bool show);
    public static ShowJosh ShowHeadMovement;

    [SerializeField] private Transform trackedObject;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform headTrackingIcon;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float alphaFadeTime = 1f;
    
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private float _alpha;

    private void Awake() {
        ShowHeadMovement += ShowIcon;
        canvasGroup.alpha = 0f;
    }

    private void OnDestroy() {
        ShowHeadMovement -= ShowIcon;
    }

    private void ShowIcon(bool show) {
        if (show)
            _alpha = 1f;
        else
            _alpha = 0f;
    }

    private void Start() {
        _startPosition = trackedObject.localPosition;
        _startRotation = cameraTransform.localRotation;
    }

    private void Update() {
        if (_alpha > 0f) {
            _alpha -= Time.unscaledDeltaTime * (1f / alphaFadeTime);
            _alpha = Mathf.Clamp01(_alpha);
            canvasGroup.alpha = _alpha;
        }
        MoveTrackedObjectPosition();
        MoveTrackedObjectIcon();
    }

    private void MoveTrackedObjectPosition() {
        Vector3 pos = TrackIRUnity.TrackIRTransform.currentPosition;
        Vector3 euler = TrackIRUnity.TrackIRTransform.currentEuler;
            
        trackedObject.localPosition = Quaternion.Euler(euler) * _startRotation * (_startPosition + pos);
    }

    private void MoveTrackedObjectIcon() {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(trackedObject.position);
        headTrackingIcon.anchoredPosition = screenPosition / canvas.scaleFactor;
    }
}
