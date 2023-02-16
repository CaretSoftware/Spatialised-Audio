using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WashingMachineVolume : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform player;
    [SerializeField] private Transform basementDoor;
    
    [SerializeField] private float doorOpenAngle;
    [SerializeField] private float doorClosedAngle;
    [SerializeField] private float maxVolumeDoorClosed;
    [SerializeField] private float firstFloorHeight = 0f;

    private Vector3 _inverseTransformPoint;
    private Vector3 _basementDoorPosition;
    private Vector3 _playerPos;
    private const float HearingDistance = .5f;
    private float _currDoorAngle;
    private float _openness;
    private float _closedDoorVolume;
    private float _maxVolume;
    private float _distance;
    private bool _outsideBasement;
    private bool _inside;
    
    private void Awake() {
        _basementDoorPosition = this.transform.position;
        _maxVolume = audioSource.volume;
    }


    private void Update() {
        _playerPos = player.position;
        _inverseTransformPoint = transform.InverseTransformPoint(_playerPos);

        _distance = Vector3.Distance(Vector3.zero, _inverseTransformPoint);

        _outsideBasement = _playerPos.z < _basementDoorPosition.z;

        _currDoorAngle = basementDoor.rotation.eulerAngles.y;
        
        _openness = Mathf.InverseLerp(doorClosedAngle, doorOpenAngle, _currDoorAngle);
        
        
        _closedDoorVolume = _outsideBasement
            ? Mathf.Lerp(maxVolumeDoorClosed, _maxVolume, _openness) 
            : _maxVolume;
        
        bool beneath = _playerPos.y < firstFloorHeight;
        
        audioSource.volume = beneath 
            ? _maxVolume 
            : _closedDoorVolume * Mathf.InverseLerp(HearingDistance,0f, _distance);
    }
}
