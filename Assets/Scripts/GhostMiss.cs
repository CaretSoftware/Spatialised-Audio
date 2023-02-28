using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMiss : MonoBehaviour {
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    
    [SerializeField, Range(0f, 1f)] private float missEffectTime = 1f;
    [SerializeField, Range(0f, 1f)] private float fadeOutEffectTime = 1f;
    
    private Transform _playerTransform;
    private MeshRenderer[] _meshRenderers;
    private Material[] _materials = new Material[3];
    private Transform _transform;
    private Material _ghostMaterial;
    private Material _leftEyeMaterial;
    private Material _rightEyeMaterial;
    
    private MaterialPropertyBlock _mpb;

    private void Awake() {
        _playerTransform = PlayerTransform.PTransform;
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Debug.Log(_meshRenderers.Length);
        

        _mpb = new MaterialPropertyBlock();
    }

    private void Start() {
        GhostMaterials ghostMaterials = GetComponent<GhostMaterials>();
        _ghostMaterial = ghostMaterials.GhostMaterial;
        _leftEyeMaterial = ghostMaterials.LeftEyeMaterial;
        _rightEyeMaterial = ghostMaterials.RightEyeMaterial;
        _materials = ghostMaterials.Materials;
    }

    public void Missed() {
        StartCoroutine(AppearAndReappear());
    }

    private bool _appearing;
    private IEnumerator AppearAndReappear() {
        _appearing = true;
        
        StartCoroutine(MissEffect());
        yield return new WaitWhile(() => _appearing);
        StartCoroutine(HideEffect());
    }

    private IEnumerator MissEffect() {
        float t = 0;
        
        while (t <= 1f) {
            _mpb.SetFloat(Alpha, Ease.InOutCubic(t));
            SetMaterialProperties(_mpb);
            t += Time.unscaledDeltaTime * (1f / missEffectTime);
            yield return null;
        }
        
        _mpb.SetFloat(Alpha, 1f);
        SetMaterialProperties(_mpb);
        _appearing = false;
    }
    
    private IEnumerator HideEffect() {
        float t = 0;

        while (t <= 1f) {
            _mpb.SetFloat(Alpha, Ease.InOutCubic(1f - t));
            SetMaterialProperties(_mpb);
            t += Time.unscaledDeltaTime * (1f / fadeOutEffectTime);
            yield return null;
        }
        
        _mpb.SetFloat(Alpha, 0);
        SetMaterialProperties(_mpb);
    }

    private void SetMaterialProperties(MaterialPropertyBlock mpb) {
        for (int i = 0; i < 3; i++) {
            _meshRenderers[i].SetPropertyBlock(mpb);
        }
    }
}
