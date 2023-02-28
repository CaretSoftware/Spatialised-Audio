using System;
using Unity.VisualScripting;
using UnityEngine;

public class GhostAudio : MonoBehaviour {
    public delegate void PlayAudio(bool missed);
    public static PlayAudio playAudio;

    [SerializeField] private float smoothTime;
    
    private Transform _ghostTransform;
    private Vector3 currentPosition;
    private Vector3 currentVelocity;

    [SerializeField] private AudioSource laughAudioSource;

    private void Awake() => playAudio += PlaySound;

    private void OnDestroy() => playAudio -= PlaySound;
    
    private void Update() {
        if (_ghostTransform == null) {
            _ghostTransform = SpawnManager.activeGhost;
            return;
        }

        currentPosition =
            Vector3.SmoothDamp(
                currentPosition, 
                _ghostTransform.position, 
                ref currentVelocity, 
                smoothTime);

        transform.position = currentPosition;
    }

    private void PlaySound(bool missed) {
        if (missed) {
            // play laugh sound
            if (!laughAudioSource.isPlaying)
                laughAudioSource.Play();
        }
    }
}
