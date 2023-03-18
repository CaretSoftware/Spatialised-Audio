using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GhostAudio : MonoBehaviour {
    public delegate void PlayAudio(bool missed);
    public static PlayAudio playAudio;
    
    public delegate void NewPos(Vector3 pos);
    public static NewPos newPosition;

    [SerializeField] private float smoothTime;
    
    private Transform _ghostTransform;
    private Vector3 _currentPosition;
    private Vector3 _currentVelocity;
    private Vector3 _targetPosition;

    [FormerlySerializedAs("laughAudioSource")] [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioClip missClip;
    [SerializeField] private AudioClip hitClip;

    private void Awake() {
        newPosition += NewPosition;
        playAudio += PlaySound;
    }

    private void OnDestroy() {
        newPosition -= NewPosition;
        playAudio -= PlaySound;
    }
    
    private void Update() {
        _currentPosition =
            Vector3.SmoothDamp(
                _currentPosition, 
                _targetPosition, 
                ref _currentVelocity, 
                smoothTime);

        transform.position = _currentPosition;
    }

    private void NewPosition(Vector3 position) {
        _targetPosition = position;
    }

    private void PlaySound(bool missed) {
        if (missed)
            ghostAudioSource.clip = missClip;
        else
            ghostAudioSource.clip = hitClip;
        
        if (!ghostAudioSource.isPlaying)
            ghostAudioSource.Play();
    }
}
