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

        // ray straight out from the center of the screen 
        _ray = cam.ViewportPointToRay(_centerScreen);
        
        if (Input.GetMouseButton(0) && SpawnManager.activeGhost != null) {
            Vector3 ghostPosition = SpawnManager.activeGhost.position;
            Vector3 cameraPosition = _cameraTransform.position;
            
            Vector3 vectorToGhost = ghostPosition - cameraPosition;
            Vector3 directionToGhost = vectorToGhost.normalized;
            Vector3 aimDirection = _cameraTransform.forward;
            
            Vector3 projectedGhostDirection = Vector3.ProjectOnPlane(directionToGhost, Vector3.up);
            
            // get rotation to get rotation in forward direction
            Quaternion fromToRotation = Quaternion.FromToRotation(projectedGhostDirection , Vector3.forward);

            // rotate ghostDirection to point in forward direction (excluding x-axis rotation)
            Vector3 rotatedGhostDirection = fromToRotation * directionToGhost;

            // rotate vector up 90 degrees
            Vector3 upRotatedGhostDirection = Quaternion.Euler(-90, 0, 0) * rotatedGhostDirection;

            // rotate the vector back to point 90 degrees perpendicular to it's original direction
            upRotatedGhostDirection = Quaternion.Inverse(fromToRotation) * upRotatedGhostDirection;
            
            // the perpendicular direction of the two vectors
            Vector3 crossRight = Vector3.Cross(upRotatedGhostDirection, directionToGhost);
            
            Debug.DrawRay(cameraPosition, crossRight);
            Debug.DrawRay(cameraPosition, upRotatedGhostDirection);
            Debug.DrawRay(cameraPosition, directionToGhost);


            Vector3 alignmentXYDot = VectorAlignment.Alignment(aimDirection, directionToGhost, crossRight);

            Debug.Log( alignmentXYDot );

            float distance = vectorToGhost.magnitude;
            
            if (Physics.Raycast(_ray, 1000f, ghostLayer, QueryTriggerInteraction.Collide)) {
                FindObjectOfType<GhostDeath>()?.Die();
                GhostAudio.playAudio?.Invoke(false);
                Debug.Log("Hit Ghost!");
            } else {
                GhostAudio.playAudio?.Invoke(true);
                FindObjectOfType<GhostMiss>()?.Missed();
            }
        }
    }
}
