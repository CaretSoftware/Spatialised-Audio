using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WeaponRotator : MonoBehaviour {
    public delegate void RecoilAmountSet(float recoil);
    public static RecoilAmountSet recoilAmountSet;

    private Vector3 _screenCenter = new Vector3(.5f, .5f, 0f);
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float rotationSpeed = 10f;
    private Transform _transform;
    [SerializeField] private Vector3 recoilRotation;
    private Quaternion _recoil;
    [SerializeField] private Transform player;

    private void Awake() {
        recoilAmountSet += RecoilAmount;
        _transform = transform;
    }

    private void OnDestroy() {
        recoilAmountSet -= RecoilAmount;
    }

    private void Update() {
        Vector3 pos = _transform.position;
        Ray ray = _camera.ViewportPointToRay(_screenCenter);

        Physics.Raycast(ray, out RaycastHit hitInfo, 100f, _layerMask);
        
        Vector3 direction = hitInfo.point - pos;

        Quaternion localRotation = Quaternion.Inverse(player.rotation) * Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
        transform.localRotation = _recoil * localRotation;
    }

    public void RecoilAmount(float recoil) {
        //Quaternion WorldRotation = Target.Transform.rotation * rotation;
        //Transforms rotation from Targets local space to world space
        _recoil = Quaternion.Slerp(quaternion.identity, Quaternion.Euler(recoilRotation), recoil);
    }
}
