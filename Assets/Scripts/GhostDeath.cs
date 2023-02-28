using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GhostDeath : MonoBehaviour {
    [SerializeField] private MeshRenderer ghostMeshRenderer;
    [SerializeField] private MeshRenderer leftEyeMeshRenderer;
    [SerializeField] private MeshRenderer rightEyeMeshRenderer;
    [SerializeField] private Rigidbody leftEyeRigidBody;
    [SerializeField] private Rigidbody rightEyeRigidBody;
    [SerializeField] private Collider leftEyeCollider;
    [SerializeField] private Collider rightEyeCollider;
    
    private Transform _playerTransform;
    private Transform _transform;
    private Material _ghostMaterial;
    private Material _leftEyeMaterial;
    private Material _rightEyeMaterial;

    private MaterialPropertyBlock _mpb;
    private MaterialPropertyBlock _eyesMpb;
    
    private bool _dying;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    public void Die() {
        if (!_dying)
            StartCoroutine(DeathEffect());
    }

    private void Awake() {
        _playerTransform = PlayerTransform.PTransform;
        _transform = transform;
        
        _mpb = new MaterialPropertyBlock();
        _eyesMpb = new MaterialPropertyBlock();
    }

    private void Start() {
        GhostMaterials ghostMaterials = GetComponent<GhostMaterials>();
        _ghostMaterial = ghostMaterials.GhostMaterial;
        _leftEyeMaterial = ghostMaterials.LeftEyeMaterial;
        _rightEyeMaterial = ghostMaterials.RightEyeMaterial;

        // Hide eyes
        _leftEyeMaterial.SetFloat(Alpha, 0f);
        _rightEyeMaterial.SetFloat(Alpha, 0f);
        // Hide body
        _mpb.SetFloat(Alpha, 0f);
        ghostMeshRenderer.SetPropertyBlock(_mpb);
        
        _eyesMpb.SetFloat(Alpha, 0f);
        leftEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
        rightEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
    }

    private IEnumerator DeathEffect() {
        _dying = true;
        float dropEyeTime = .8f;
        bool droppedEyes = false;
        float t = 0f;

        // show eyes TODO animate scale Ease.InElastic/InBack?
        _eyesMpb.SetFloat(Alpha, 1f);
        leftEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
        rightEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
        
        // show ghost
        _mpb.SetFloat(Alpha, 1f);
        ghostMeshRenderer.SetPropertyBlock(_mpb);
        
        Vector3 lookDir = _playerTransform.position - _transform.position;
        lookDir.y = 0f;
        lookDir.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
        
        while (t < 1f) {
            transform.rotation = Quaternion.Lerp(_transform.rotation, targetRotation, t);
            
            float dissolve = Mathf.Lerp(-.2f, .99f, t);
            
            _mpb.SetFloat(DissolveAmount, dissolve);
            ghostMeshRenderer.SetPropertyBlock(_mpb);
            if (t >= dropEyeTime && !droppedEyes) {
                droppedEyes = true;
                leftEyeCollider.enabled = true;
                rightEyeCollider.enabled = true;
                leftEyeRigidBody.isKinematic = false;
                rightEyeRigidBody.isKinematic = false;
            }
            
            t += Time.deltaTime;
            yield return null;
        }
        _mpb.SetFloat(DissolveAmount, .99f);
        ghostMeshRenderer.SetPropertyBlock(_mpb);

        t = 0f;

        float secondsToFade = 2f;
        while (t < 1f) {
            float alpha = Mathf.Lerp(1f, 0f, t);

            _eyesMpb.SetFloat(Alpha, alpha);
            leftEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
            rightEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
            
            t += Time.deltaTime * (1f / secondsToFade);
            yield return null;
        }
        
        _eyesMpb.SetFloat(Alpha, 0f);
        leftEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
        rightEyeMeshRenderer.SetPropertyBlock(_eyesMpb);
        
        _dying = false;
    }
}
