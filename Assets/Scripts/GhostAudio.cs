using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GhostAudio : MonoBehaviour {
    public enum Clip {
        Laugh,
        Dissolve,
        HeartBeat,
    }
    public delegate void PlayAudio(Clip clip);
    public static PlayAudio playAudio;
    
    public delegate void NewPos(Vector3 pos);
    public static NewPos newPosition;

    [SerializeField] private float smoothTime;
    
    private Vector3 _currentPosition;
    private Vector3 _currentVelocity;
    private Vector3 _targetPosition;

    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private AudioSource heartBeatSensor;
    [SerializeField] private AudioClip laughClip;
    [SerializeField] private AudioClip dissolveClip;

    private void Awake() {
        newPosition += NewPosition;
        playAudio += PlaySound;
        _currentPosition = transform.position;
        _targetPosition = _currentPosition;
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

    private void PlaySound(Clip clip) {
        switch (clip) {
            case Clip.Dissolve:
                ghostAudioSource.clip = dissolveClip;
                break;
            case Clip.Laugh:
                ghostAudioSource.clip = laughClip;
                break;
            case Clip.HeartBeat:
                if (!heartBeatSensor.isPlaying)
                    heartBeatSensor.Play();
                heartBeatSensor.spatializePostEffects = true;
                heartBeatSensor.spatializePostEffects = false;
                return;
        }

        if (!ghostAudioSource.isPlaying)
            ghostAudioSource.Play();
    }
}
