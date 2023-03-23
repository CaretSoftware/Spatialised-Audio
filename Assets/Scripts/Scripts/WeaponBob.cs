using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponBob : MonoBehaviour {
    public delegate void Recoil();
    public static Recoil recoil;
    
    public delegate void IsAtEnd();
    public static IsAtEnd atEnding;

    [SerializeField] private Transform _player;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float xAxisSway;
    [SerializeField] private float yAxisSway;
    [SerializeField] private float swaySpeed = 5f;
    [SerializeField] private Vector3 recoilVector = Vector3.back  *.3f;
    [SerializeField, Range(0f, 1f)] private float smoothRecoilRecoveryTime;
    private Transform _transform;
    private Vector3 _recoil;
    private Vector3 _localRestPosition;
    private Vector3 _currentRecoilVelocity;
    private float _currentPlayerVelocity;
    private float _playerRefVelocity;
    private float _time;
    private float _speed = 1f;
    private bool _atEnd;
    
    private void Awake() {
        recoil += PerformRecoil;
        atEnding += AtEnd;
        _transform = transform;
        _localRestPosition = _transform.localPosition;
    }

    private void OnDestroy() {
        recoil -= PerformRecoil;
        atEnding -= AtEnd;
    }

    private void Update() {
        WeaponSway();
        DampenRecoil();
        if (_atEnd)
            DecreaseMagnitude();
    }

    private void AtEnd() {
        _atEnd = true;
    }

    private void DecreaseMagnitude() {
        _speed -= Time.unscaledDeltaTime;
        _speed = Mathf.Clamp01(_speed);
    }

    private void WeaponSway() {
        _time += Time.deltaTime;
        float time = _time * swaySpeed;
        float x = Mathf.Sin(time) * xAxisSway;
        float y = -Mathf.Abs(Mathf.Cos(time) * yAxisSway);
        _currentPlayerVelocity = Mathf.SmoothDamp(_currentPlayerVelocity, _characterController.velocity.magnitude * _speed,
            ref _playerRefVelocity, .2f);
        Vector3 sway = new Vector3(x, y, 0f) * _currentPlayerVelocity;
        _transform.localPosition = _localRestPosition + sway + _recoil;
    }

    private void DampenRecoil() {
        _recoil = Vector3.SmoothDamp(_recoil, Vector3.zero, ref _currentRecoilVelocity, smoothRecoilRecoveryTime);
        WeaponRotator.recoilAmountSet?.Invoke(_recoil.magnitude);
    }
    
    private void PerformRecoil() {
        _recoil = recoilVector;
    }
}
