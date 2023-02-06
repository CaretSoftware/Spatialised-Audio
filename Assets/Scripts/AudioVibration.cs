using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVibration : MonoBehaviour {
    private AudioSource _audioSource;
    public float[] _samples = new float[512];
    [SerializeField, Range(0, 511)] private int frequencyProbe;
    [SerializeField] private float rotAngle;
    private Transform _transform;
    private Quaternion _startRot;
    [SerializeField] private float magnitude = 1f;
    
    
    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
        _transform = transform;
        _startRot = _transform.rotation;
    }

    private void Update() {
        GetSpectrumAudioSource();
        Quaternion rot = _transform.rotation;
        Quaternion toRot = Quaternion.Euler(0f, 0f, rotAngle);
        float samples = _samples[frequencyProbe] * magnitude; // smoothDamp this
        Quaternion rotation = Quaternion.Slerp(_startRot,  _startRot * toRot, samples);
        _transform.rotation = rotation;
    }

    private void GetSpectrumAudioSource() {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }
}
