using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashingMachineVolume : MonoBehaviour {
    private float _maxVolume;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform player;
    private float _hearingDistance = .5f;
    private Vector3 _basementDoorPosition;
    private bool _inside;
    private float _minVolume = .27f;
    [SerializeField] private Transform basementDoor;
    [SerializeField] private float doorOpenAngle;
    [SerializeField] private float doorClosedAngle;
    [SerializeField] private float maxVolumeDoorClosed;

    private void Awake() {
        // GetComponent<SphereCollider>().radius = hearingDistance;
        _basementDoorPosition = this.transform.position;
        _maxVolume = audioSource.volume;
    }

    public float dist;
    public float debug;
    public float angle;
    private void Update() {
        Vector3 playerPos = player.position;
        Vector3 inverseTransformPoint = transform.InverseTransformPoint(playerPos);

        dist = Vector3.Distance(Vector3.zero, inverseTransformPoint);
        debug = _maxVolume * Mathf.InverseLerp(_hearingDistance, 0f, dist);

        bool outsideBasement = playerPos.z < _basementDoorPosition.z;

        float doorAngle = basementDoor.rotation.eulerAngles.y;
        
        angle = doorAngle;
        
        float openness = Mathf.InverseLerp(doorClosedAngle, doorOpenAngle, doorAngle);
        
        debug = Mathf.Lerp(maxVolumeDoorClosed, _maxVolume,  Ease.OutExpo(openness));
        
        float maxVolume = outsideBasement
            ? Mathf.Lerp(maxVolumeDoorClosed, _maxVolume, openness) 
            : _maxVolume;
        
        bool beneath = playerPos.y < -0.021;
        
        audioSource.volume = beneath 
            ? _maxVolume 
            : maxVolume * Mathf.InverseLerp(_hearingDistance,0f, dist);
    }

    // public float dist;
    // private void Update() {
    //     playerY = player.position.y;
    //     if (PlayerIsDownstairs()) {
    //         audioSource.volume = _maxVolume;
    //         return;
    //     }
    //     
    //     float distance = Vector3.Distance(_basementDoorPosition, player.position);
    //     dist = distance;
    //     
    //     audioSource.volume = _maxVolume * Mathf.InverseLerp(hearingDistance,0f, distance);
    // }
// 
    // private void OnTriggerEnter(Collider other) {
    //     if (!other.CompareTag("Player")) return;
    //     
    //     _inside = true;
    // }
// 
    // private void OnTriggerExit(Collider other) {
    //     if (!other.CompareTag("Player")) return;
// 
    //     _inside = PlayerIsDownstairs();
    //     Debug.Log($"exit: {!_inside}");
    //     if (!_inside) 
    //         audioSource.volume = _minVolume;
    // }
// 
    // public float playerY;
    // 
    // private bool PlayerIsDownstairs() {
    //     return player.position.y < -0.021;
    // }
}
