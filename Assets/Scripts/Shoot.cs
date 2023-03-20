using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
    private readonly Vector3 _centerScreen = new Vector3(.5f, .5f, 0f);
    [SerializeField] private Camera cam;
    private LayerMask _shootLayerMask;
    private Transform _cameraTransform;
    private Ray _ray;

    private int _roundNumber;
    private float _timeFirstShot;
    private float _timeLastShot;

    private CSVWriter _csvWriter;
    //private bool _timerStarted;
    private float _totalElapsedTime;
    private float _roundElapsedTime;
    private const float fullyCharged = 1f; 
    private float _refractoryPeriod = 1f;
    private float _charge = -1f;
    [SerializeField] private MeshRenderer chargeLight;
    private MaterialPropertyBlock _mpb;
    private static readonly int Light1 = Shader.PropertyToID("_Light");

    private bool _charging = true;
    
    private float _time;

    private void Awake() {
        _shootLayerMask = LayerMask.GetMask("Default", "Ghost");
        _mpb = new MaterialPropertyBlock();
        Material material = chargeLight.material;
        chargeLight.material = material;
    }

    private void Start() {
        _cameraTransform = cam.transform;
        _csvWriter = FindObjectOfType<CSVWriter>();
    }

    public bool UpdateMe(bool timed = true) {
        _charge += Time.deltaTime;
        
        if (timed)
            _time += Time.deltaTime;

        if (_charging && _charge > 0f) {
            _charging = false;
            PlayWeaponSounds.playCharge?.Invoke();
        }
        
        if (_charge >= 1f && _charge <= 2.1f) {
            _mpb.SetFloat(Light1, _charge - 1f);
            chargeLight.SetPropertyBlock(_mpb);
        }
            
        _ray = cam.ViewportPointToRay(_centerScreen);

        if (timed) {
            _totalElapsedTime = _time;
            _roundElapsedTime = _time - _timeLastShot;
            Timer.UpdateTimerSeconds?.Invoke(_roundElapsedTime);
            Timer.UpdateTimerMinutes?.Invoke(_roundElapsedTime);
        }
        
        if (Input.GetMouseButtonDown(0) && _charge >= fullyCharged) {
            if (timed) {
                RoundCounter.roundText?.Invoke(CSVWriter.RoundNumber);
                if (CSVWriter.RoundNumber >= 14) { // END
                    GameState.maxRounds?.Invoke();
                }
            }
                
            _charge = -_refractoryPeriod;
            _charging = true;
                
            //_timerStarted = true; // TODO
            
            _mpb.SetFloat(Light1, 0f);
            chargeLight.SetPropertyBlock(_mpb);

            ShotVisuals();
            TakeShot();
            return true;
        }

        return false;
    }

    private void ShotVisuals() {
        WeaponBob.recoil?.Invoke();
        LaserLine.showLineRenderer?.Invoke();
        PlayWeaponSounds.playShot?.Invoke();
    }

    private void TakeShot() {
        Transform ghost = SpawnManager.activeGhost;
        if (ghost == null) return;

        _timeLastShot = _time;
        
        Vector3 ghostPosition = ghost.position;
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

        if (_roundNumber++ < 1) {
            _timeFirstShot = _time;
            //return;
        }

        int subjectNumber = CSVWriter.SubjectNumber;
        //_totalElapsedTime = Time.time - _timeFirstShot;
        int roundNumber = CSVWriter.RoundNumber;
        //_roundElapsedTime = _totalElapsedTime - _timeLastShot;
        float precision = alignmentXYDot.z;
        float precisionX = alignmentXYDot.x;
        float precisionY = alignmentXYDot.y;
        float distance = vectorToGhost.magnitude;
        Physics.Raycast(_ray, out RaycastHit hitInfo, 100f,  _shootLayerMask, QueryTriggerInteraction.Collide);
        bool hit = hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Ghost");

        int playerFloor = 1; // TODO Calculate floors !
        int ghostFloor = 1;

        ShotData shotData = new ShotData(subjectNumber, _totalElapsedTime, roundNumber, _roundElapsedTime,
            precision, precisionX, precisionY, distance, hit, playerFloor, ghostFloor);

        _csvWriter.AppendCSV(shotData);
        Invoke(nameof(SaveSymbol), 2f);

        if (hit) {
            GhostDeath.died?.Invoke();
            GhostAudio.playAudio?.Invoke(GhostAudio.Clip.Dissolve);
        } else {
            GhostMiss.miss?.Invoke();
            GhostAudio.playAudio?.Invoke(GhostAudio.Clip.Laugh);
        }
    }
    
    private void SaveSymbol() {
        ImageProgressFill.ProgressFillStart?.Invoke();
    }

}
