using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPractise : MonoBehaviour {
    public delegate void GotHit();
    public static GotHit gotHit;
    
    public delegate void Pull();
    public static Pull pullTarget;

    private Vector3 _targetPosition;
    private Vector3 _currentPosition;
    private Vector3 _currentVelocity;
    private Collider _collider;
    
    [SerializeField] private Vector3[] targetPositions = new Vector3[4];

    [SerializeField] private float step1 = 5f;
    [SerializeField] private float step2 = 7f;
    [SerializeField] private bool _hit;
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private Transform _target;

    private float _shakeTime;
    private int _hits;

    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    private MaterialPropertyBlock _mpb;
    private MeshRenderer _meshRenderer;

    private void Awake() {
        _currentPosition = transform.position;
        _collider = GetComponent<Collider>();
        gotHit += HasBeenHit;
        pullTarget += PullTarget;
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        Material mat = _meshRenderer.material;
        _meshRenderer.material = mat;
        _mpb = new MaterialPropertyBlock();
    }

    private void PullTarget() {
        if (_hits == 0) {
            _hits++;
            GhostAudio.newPosition?.Invoke(targetPositions[_hits]);
            StartCoroutine(ScaleUp());
        }
    }

    private IEnumerator ScaleUp() {
        float t = 0f;

        Transform transf = transform;
        Vector3 currentScale = transf.localScale;
        Vector3 targetScale = Vector3.one;
        
        while (t < 1f) {
            float e = Ease.OutBack(t);
            transf.localScale = Vector3.LerpUnclamped(currentScale, targetScale, e);
            t += Time.deltaTime * 2f;
            yield return null;
        }
        
        transf.localScale = targetScale;
    }
    
    private IEnumerator ScaleDown() {
        float t = 0f;

        Transform transf = transform;
        Vector3 currentScale = Vector3.one;
        Vector3 targetScale = Vector3.zero;
        
        while (t < 1f) {
            float e = Ease.InBack(t);
            transf.localScale = Vector3.LerpUnclamped(currentScale, targetScale, e);
            t += Time.deltaTime * 2f;
            yield return null;
        }
        
        transf.localScale = targetScale;
    }

    private void Update() {
        _hits = Math.Clamp(_hits, 0, targetPositions.Length - 1);
        _currentPosition = Vector3.SmoothDamp(_currentPosition, targetPositions[_hits], ref _currentVelocity, .1f);
        transform.position = _currentPosition;
        _collider.enabled = Vector3.Distance(_currentPosition, targetPositions[_hits]) < .01f &&
                            _hits < 4;

        if (_hit || _shakeTime > 0f) {
            if (_hit) {
                _hit = false;
                _shakeTime = 1f;
                SetAlpha(1);
            }

            ShakeTarget();
            
            _shakeTime -= Time.deltaTime;

            if (_shakeTime <= 0f) {
                TutorialState.hits(++_hits - 1);
                GhostAudio.newPosition?.Invoke(targetPositions[_hits]);

                if (_hits >= 4) {
                    StartCoroutine(ScaleDown());
                    _collider.enabled = false;
                }
                else
                    StartCoroutine(FadeAlpha(0f));
            }
        }
        
        void ShakeTarget() {
            float e = Ease.InExpo(_shakeTime);
            float x = Mathf.Sin(Time.time * step1) * shakeMagnitude * e;
            x += Mathf.Sin(Time.time * step2) * shakeMagnitude * e;
            
            float y = Mathf.Cos(Time.time * 1.1f * step1) * shakeMagnitude * e;
            y += Mathf.Cos(Time.time * 1.4f * step2) * shakeMagnitude * e;
 
            float z = Mathf.Cos(Time.time * 1.4f * step1) * shakeMagnitude * e;
            z += Mathf.Cos(Time.time * 1.3f * step2) * shakeMagnitude * e;

            _target.localPosition = new Vector3(x, y, z);
        }
    }

    private IEnumerator FadeAlpha(float targetAlpha) {
        float t = 0f;

        while (t < 1f) {
            SetAlpha(1f - t);
            t += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0f);
    }
    
    private void SetAlpha(float alpha) {
        _mpb.SetFloat(Alpha, alpha);
        _meshRenderer.SetPropertyBlock(_mpb);
    }

    private void OnDestroy() {
        gotHit -= HasBeenHit;
        pullTarget -= PullTarget;
    }

    private void HasBeenHit() {
        _hit = true;
    }
}
