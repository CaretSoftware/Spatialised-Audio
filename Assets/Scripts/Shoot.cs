using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Shoot : MonoBehaviour {
    private readonly Vector3 _centerScreen = new Vector3(.5f, .5f, 0f);
    
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private Camera cam;
    
    private Transform _cameraTransform;
    private Ray _ray;

    private void Awake() {
        _cameraTransform = cam.transform;
    }

    private void Update() {

        _ray = cam.ViewportPointToRay(_centerScreen);
        
        if (Input.GetMouseButton(0)) {
            Vector3 ghostPosition = GhostManager.ActiveGhost.position;
            Vector3 vectorToGhost = ghostPosition - _cameraTransform.position;
            Vector3 directionToGhost = vectorToGhost.normalized;
            
            Vector3 cameraUp = _cameraTransform.up;
            Vector3 cameraRight = _cameraTransform.right;
            Vector3 cameraForward = _cameraTransform.forward;

            Vector3 projectedYVector = Vector3.ProjectOnPlane(directionToGhost, cameraUp).normalized;
            Vector3 projectedXVector = Vector3.ProjectOnPlane(directionToGhost, cameraRight).normalized;

            Vector3 cameraYCross = Vector3.Cross(cameraForward, cameraUp); // camera.right
            Vector3 cameraXCross = Vector3.Cross(cameraRight, cameraForward); // cam.up ?

            
            Vector3 projectedXCamVector = Vector3.ProjectOnPlane(_ray.direction.normalized, cameraUp).normalized;
            Vector3 projectedYCamVector = Vector3.ProjectOnPlane(_ray.direction.normalized, cameraRight).normalized;

            //bool beyond90DegreesY = Vector3.Dot(projectedXVector, cameraForward) < 0f; 
            //bool beyond90DegreesX = Vector3.Dot(projectedYVector, cameraForward) < 0f; 

            float dotX = Vector3.Dot(projectedYVector, cameraRight);// cameraYCross); 
            float dotY = Vector3.Dot(projectedXVector, cameraUp);//cameraXCross);
            
            // if (beyond90DegreesX)
            //     dotY = 1f - dotY;
            // if (beyond90DegreesY)
            //     dotX = 1f - dotX;
            
            float distance = vectorToGhost.magnitude;
            
            //Debug.Log($"{dotX} {dotY}");
        }
        
        if (Input.GetMouseButton(0)) {
            //Debug.DrawRay(cam.transform.position, _ray.direction * 5f, Color.red, Time.deltaTime);

            if (Physics.Raycast(_ray, 1000f, ghostLayer, QueryTriggerInteraction.Collide)) {
                Debug.Log("Hit Ghost!");
            }
        }
    }
}
