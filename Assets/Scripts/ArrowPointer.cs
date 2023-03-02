using System;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPointer : MonoBehaviour {
    public Transform targetObject;

    public RectTransform arrowTransform;

    public Canvas canvas;

    public float arrowOffset = 50f;

    // public float arrowRotationSpeed = 180f;

    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        Point();
    }

    void Point() {
        // Check if the target object is visible on screen

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(targetObject.position);

        if (viewportPoint.z < 0f 
            || viewportPoint.x < 0f 
            || viewportPoint.x > 1f 
            || viewportPoint.y < 0f 
            || viewportPoint.y > 1f) {
            
            arrowTransform.gameObject.SetActive(true);

            // Calculate the arrow position and rotation

            Vector2 arrowScreenPosition = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f) *
                                          canvas.GetComponent<RectTransform>().sizeDelta;

            arrowScreenPosition.x += arrowOffset * Mathf.Sign(arrowScreenPosition.x);
            arrowScreenPosition.y += arrowOffset * Mathf.Sign(arrowScreenPosition.y);

            //Vector2 upperLeft = Vector2.zero;
            Vector2 lowerRight = mainCamera.ViewportToScreenPoint(new Vector3(1, 1, 0));
            
            arrowScreenPosition.x = Mathf.Clamp(arrowScreenPosition.x, -lowerRight.x * .5f, lowerRight.x * .5f);
            arrowScreenPosition.y = Mathf.Clamp(arrowScreenPosition.y, -lowerRight.y * .5f, lowerRight.y * .5f);
            
            arrowTransform.anchoredPosition = arrowScreenPosition;

            float arrowRotation = Mathf.Atan2(arrowScreenPosition.y, arrowScreenPosition.x) * Mathf.Rad2Deg;

            arrowTransform.rotation = Quaternion.Euler(0f, 0f, arrowRotation);


            // Spin the arrow
            // arrowTransform.Rotate(Vector3.forward, arrowRotationSpeed * Time.deltaTime);
        }
        else {
            // Hide the arrow if the target object is visible on screen
            // arrowTransform.gameObject.SetActive(false);
        }
    }
}