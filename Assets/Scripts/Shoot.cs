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

    private CSVWriter _csvWriter;
    private bool _timerStarted;
    private float _totalElapsedTime;
    private float _roundElapsedTime;
    private const float _refractoryPeriod = 1f;
    private float _charge = -_refractoryPeriod;
    [SerializeField] private MeshRenderer chargeLight;
    private MaterialPropertyBlock _mpb;
    private static readonly int Light1 = Shader.PropertyToID("_Light");

    private void Awake() {
        _mpb = new MaterialPropertyBlock();
        Material material = chargeLight.material;
        chargeLight.material = material;
    }

    private void Start() {
        _cameraTransform = cam.transform;
        _csvWriter = FindObjectOfType<CSVWriter>();
    }

    private void Update() {
        _charge += Time.deltaTime;
        
        if (_charge >= 0f && _charge <= 1.1f) {
            _mpb.SetFloat(Light1, _charge);
            chargeLight.SetPropertyBlock(_mpb);
        }
            
        // ray straight out from the center of the screen 
        _ray = cam.ViewportPointToRay(_centerScreen);

        if (_timerStarted) {
            _totalElapsedTime = Time.time - _timeFirstShot;
            _roundElapsedTime = Time.time - _timeLastShot;
            Timer.UpdateTimerSeconds?.Invoke(_roundElapsedTime);
            Timer.UpdateTimerMinutes?.Invoke(_roundElapsedTime);
        }
        
        if (Input.GetMouseButtonDown(0) && _charge >= 1f && SpawnManager.activeGhost != null) {
            _charge = -_refractoryPeriod;
            
            _mpb.SetFloat(Light1, 0f);
            chargeLight.SetPropertyBlock(_mpb);
            
            _timerStarted = true; // TODO

            ShotVisuals();
            TakeShot();
        }
    }

    private void ShotVisuals() {
        WeaponBob.recoil?.Invoke();
        LaserLine.showLineRenderer?.Invoke();
        PlayShotSounds.playShot?.Invoke();
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

        if (_roundNumber++ < 1) {
            _timeFirstShot = Time.time;
            return;
        }

        int subjectNumber = CSVWriter.SubjectNumber;
        //_totalElapsedTime = Time.time - _timeFirstShot;
        int roundNumber = CSVWriter.RoundNumber;
        //_roundElapsedTime = _totalElapsedTime - _timeLastShot;
        float precision = alignmentXYDot.z;
        float precisionX = alignmentXYDot.x;
        float precisionY = alignmentXYDot.y;
        float distance = vectorToGhost.magnitude;
        bool hit = Physics.Raycast(_ray, 1000f, ghostLayer, QueryTriggerInteraction.Collide);
        int playerFloor = 1; // TODO Calculate floors !
        int ghostFloor = 1;

        ShotData shotData = new ShotData(subjectNumber, _totalElapsedTime, roundNumber, _roundElapsedTime,
            precision, precisionX, precisionY, distance, hit, playerFloor, ghostFloor);

        _csvWriter.AppendCSV(shotData);
        Invoke(nameof(SaveSymbol), 2f);

        
        if (hit) {
            FindObjectOfType<GhostDeath>()?.Die();
            GhostAudio.playAudio?.Invoke(false);
            Debug.Log("Hit Ghost!");
        } else {
            GhostAudio.playAudio?.Invoke(true);
            FindObjectOfType<GhostMiss>()?.Missed();
        }
    }
    
    private void SaveSymbol() {
        ImageProgressFill.ProgressFillStart?.Invoke();
    }

}
