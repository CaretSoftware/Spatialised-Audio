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

    private int _roundNumber;
    private float _timeFirstShot;
    private float _timeLastShot;
    

    private void Awake() {
        _cameraTransform = cam.transform;
    }

    private void Update() {

        // ray straight out from the center of the screen 
        _ray = cam.ViewportPointToRay(_centerScreen);
        
        if (Input.GetMouseButton(0) && SpawnManager.activeGhost != null) {
            ShotVisuals();
            TakeShot();
        }
    }

    private void ShotVisuals() {
        Debug.Log("SHOT VISUALS");
    }

    private void TakeShot() {
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

        _timeLastShot = Time.time;

        if (_roundNumber++ > 0) {
            _timeFirstShot = Time.time;
            return;
        }

        int subjectNumber = CSVWriter.SubjectNumber;
        float totalElapsedTime = Time.time - _timeFirstShot;
        int roundNumber = CSVWriter.RoundNumber;
        float roundElapsedTime = totalElapsedTime - _timeLastShot;
        float precision = alignmentXYDot.z;
        float precisionX = alignmentXYDot.x;
        float precisionY = alignmentXYDot.y;
        float distance = vectorToGhost.magnitude;
        bool hit = Physics.Raycast(_ray, 1000f, ghostLayer, QueryTriggerInteraction.Collide);
        int playerFloor = 1;
        int ghostFloor = 1;

        ShotData shotData = new ShotData(subjectNumber, totalElapsedTime, roundNumber, roundElapsedTime,
            precision, precisionX, precisionY, distance, hit, playerFloor, ghostFloor);
        
        if (hit) {
            FindObjectOfType<GhostDeath>()?.Die();
            GhostAudio.playAudio?.Invoke(false);
            Debug.Log("Hit Ghost!");
        } else {
            GhostAudio.playAudio?.Invoke(true);
            FindObjectOfType<GhostMiss>()?.Missed();
        }
    }
}
