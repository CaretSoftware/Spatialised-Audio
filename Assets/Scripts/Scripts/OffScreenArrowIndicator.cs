using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class OffScreenArrowIndicator : MonoBehaviour {
    private const float TAU = 2 * Mathf.PI;
    private const float quarterClockwiseTurn = -.25f * TAU;
    
    [SerializeField] private float screenBoundOffset = 0.9f;
    
    public RectTransform arrowTransform;
    public RectTransform iconTransform;
    private Camera _mainCamera;
    private Canvas _canvas;
    public bool showIndicator;
    private float _currentRotation;
    private float _currentVelocity;
    public float smoothTime = .15f;
    public bool hideOnScreen = true;
    
    private void Start() {
        _mainCamera = Camera.main;
        _canvas = GetComponent<Canvas>();
        GhostAudio.playAudio += ShowGhostIndicator;
        SpawnManager.respawnGhost += HideGhostIndicator;
    }

    private void OnDestroy() {
        GhostAudio.playAudio -= ShowGhostIndicator;
        SpawnManager.respawnGhost -= HideGhostIndicator;
    }

    private void ShowGhostIndicator(bool missed) {
        showIndicator = true;
    }
    
    private void HideGhostIndicator() {
        showIndicator = false;
    }
    
    // https://github.com/jinincarnate/off-screen-indicator/tree/master
    private void Update() {
        if (SpawnManager.activeGhost == null)
            return;

        if (!showIndicator) {
            arrowTransform.gameObject.SetActive(false);
            iconTransform.gameObject.SetActive(false);
            return;
        }
        
        Transform targetObject = SpawnManager.activeGhost;
        Transform cam = _mainCamera.transform;
        Vector3 targetVector = targetObject.position;
        Vector3 direction = targetVector - cam.position;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(targetVector);
        Vector3 screenCentre = new Vector3(Screen.width * .5f, Screen.height * .5f, 0f);
        Vector3 screenBounds = screenCentre * screenBoundOffset;
        float alignment = Vector3.Dot(direction.normalized, cam.forward);
        float angle;

        // scale parameters by target distance
        float distance = direction.magnitude;
        float minDistance = 4f;
        float maxDistance = 20f;
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        Vector3 scale = Vector3.Lerp(Vector3.one, Vector3.one * .4f, t);
        
        arrowTransform.localScale = scale;
        
        if (WithinScreen(screenPosition)) {
            if (hideOnScreen) {
                
                arrowTransform.gameObject.SetActive(false);
                iconTransform.gameObject.SetActive(false);
    
                // methods below shows indicator also when it's on screen
                return;
            }
            
            // Our screenPosition's origin is screen's bottom-left corner.
            // But we have to get the arrow's screenPosition and rotation with respect to screenCentre.
            // we have to move - it back only if we have anchor position in the middle
            // if we have the anchor position in the bottom left we can use the screen position
            screenPosition -= screenCentre;

            // smoothly set the rotation to rotate the arrow pointing down (-Pi / 2)
            _currentRotation = Mathf.SmoothDamp(_currentRotation, quarterClockwiseTurn, ref _currentVelocity, smoothTime);

            SetAnchoredPositionAndRotation(arrowTransform, screenPosition, _currentRotation);
            SetAnchoredPositionAndRotation(iconTransform, iconTransform.anchoredPosition, Mathf.Sin(Time.time * 5f) * .2f);
            return;
        }
        
        arrowTransform.gameObject.SetActive(true);
        iconTransform.gameObject.SetActive(true);
        
        // Our screenPosition's origin is screen's bottom-left corner.
        // But we have to get the arrow's screenPosition and rotation with respect to screenCentre.
        screenPosition -= screenCentre;
        
        // Angle between the x-axis (bottom of screen) and a vector starting at zero(bottom-left corner of screen) and terminating at screenPosition.
        angle = Mathf.Atan2(screenPosition.y, screenPosition.x);
        // Slope of the line starting from zero and terminating at screenPosition.
        float slope = Mathf.Tan(angle);

        // Two point's line's form is (y2 - y1) = m (x2 - x1) + c, 
        // starting point (x1, y1) is screen botton-left (0, 0),
        // ending point (x2, y2) is one of the screenBounds,
        // m is the slope
        // c is y intercept which will be 0, as line is passing through origin.
        // Final equation will be y = mx.
        if (screenPosition.x > 0) {
            // Keep the x screen position to the maximum x bounds and
            // find the y screen position using y = mx.
            screenPosition = new Vector3(screenBounds.x, screenBounds.x * slope, 0);
        }
        else {
            screenPosition = new Vector3(-screenBounds.x, -screenBounds.x * slope, 0);
        }

        // In case the y ScreenPosition exceeds the y screenBounds 
        if (screenPosition.y > screenBounds.y) {
            // Keep the y screen position to the maximum y bounds and
            // find the x screen position using x = y/m.
            screenPosition = new Vector3(screenBounds.y / slope, screenBounds.y, 0);
        }
        else if (screenPosition.y < -screenBounds.y) {
            screenPosition = new Vector3(-screenBounds.y / slope, -screenBounds.y, 0);
        }

        if (alignment < 0f) {
            screenPosition *= -1f;
            angle += Mathf.PI;
        }

        // set smooth damp values
        _currentVelocity = 0f;
        // prevent angle from spinning the long way around to right itself
        _currentRotation = angle < Mathf.PI && angle > .5f * Mathf.PI ? angle - TAU : angle;

        SetAnchoredPositionAndRotation(arrowTransform, screenPosition, angle);
        SetAnchoredPositionAndRotation(iconTransform, iconTransform.anchoredPosition, Mathf.Sin(Time.time * 5f) * .2f);
    }

    private bool WithinScreen(Vector3 screenPosition) {
        return (screenPosition.z > 0
                && screenPosition.x > 0
                && screenPosition.x < Screen.width
                && screenPosition.y > 0
                && screenPosition.y < Screen.height);
    }

    private void SetAnchoredPositionAndRotation(RectTransform rectTransform, Vector3 anchoredPosition, float degreesInRadians) {
        rectTransform.anchoredPosition = anchoredPosition / _canvas.scaleFactor ;//Sets the position of the indicator on the screen.
        rectTransform.rotation = Quaternion.Euler(0, 0, degreesInRadians * Mathf.Rad2Deg); // Sets the rotation for the arrow indicator.
    }
}
