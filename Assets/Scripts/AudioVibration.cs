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

    private float _sample;
    private float _velocity;
    [SerializeField] private float dampening = .1f;
    [SerializeField] private int numberOfSamples = 10;
    
    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
        _transform = transform;
        _startRot = _transform.rotation;
    }

    private void Update() {
        GetSpectrumAudioSource();
        Quaternion rot = _transform.rotation;
        Quaternion toRot = Quaternion.Euler(0f, 0f, rotAngle);
        //float s1 = _samples[frequencyProbe] * magnitude;

        float sum = 0f;
        for (int i = 0; i < numberOfSamples; i++)
            sum += _samples[(frequencyProbe + i) % _samples.Length];
        
        float average = sum / numberOfSamples;

        _sample = Mathf.SmoothDamp(_sample, average * magnitude, ref _velocity, dampening);
        Quaternion rotation = Quaternion.Slerp(_startRot,  _startRot * toRot, _sample);
        _transform.rotation = rotation;
    }

    private void GetSpectrumAudioSource() {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }
}
