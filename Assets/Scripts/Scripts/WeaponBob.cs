using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponBob : MonoBehaviour {
    public delegate void Recoil();
    public static Recoil recoil;

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
    
    private void Awake() {
        recoil += PerformRecoil;
        _transform = transform;
        _localRestPosition = _transform.localPosition;
    }

    private void OnDestroy() {
        recoil -= PerformRecoil;
    }

    private void Update() {
        WeaponSway();
        DampenRecoil();
    }

    private void WeaponSway() {
        float time = Time.time * swaySpeed;
        float x = Mathf.Sin(time) * xAxisSway;
        float y = -Mathf.Abs(Mathf.Cos(time) * yAxisSway);
        _currentPlayerVelocity = Mathf.SmoothDamp(_currentPlayerVelocity, _characterController.velocity.magnitude,
            ref _playerRefVelocity, .2f);
        Vector3 sway = new Vector3(x, y, 0f) * _currentPlayerVelocity;
        _transform.localPosition = _localRestPosition + sway + _recoil;
    }

    private void DampenRecoil() {
        _recoil = Vector3.SmoothDamp(_recoil, Vector3.zero, ref _currentRecoilVelocity, smoothRecoilRecoveryTime);
        WeaponRotator.recoilAmountSet?.Invoke(_currentRecoilVelocity.magnitude);
    }
    
    private void PerformRecoil() {
        _recoil = recoilVector;
    }
}
