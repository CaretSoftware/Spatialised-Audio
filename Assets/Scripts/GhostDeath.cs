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
    [SerializeField] private Transform playerTransform;
    
    private Transform _transform;
    private Material _ghostMaterial;
    private Material _leftEyeMaterial;
    private Material _rightEyeMaterial;

    private MaterialPropertyBlock _mpb;
    
    private bool _dying;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    private void Awake() {
        _transform = transform;
        _ghostMaterial = ghostMeshRenderer.material;
        _leftEyeMaterial = leftEyeMeshRenderer.material;
        _rightEyeMaterial = rightEyeMeshRenderer.material;
        
        _mpb = new MaterialPropertyBlock();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            Die();
    }

    public void Die() {
        if (!_dying)
            StartCoroutine(DeathEffect());
    }

    private IEnumerator DeathEffect() {
        _dying = true;
        float dropEyeTime = .8f;
        bool droppedEyes = false;
        float t = 0f;

        Vector3 lookDir = playerTransform.position - _transform.position;
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

            _leftEyeMaterial.SetFloat(Alpha, alpha);
            _rightEyeMaterial.SetFloat(Alpha, alpha);
            t += Time.deltaTime * (1f / secondsToFade);
            yield return null;
        }
        
        _leftEyeMaterial.SetFloat(Alpha, 0f);
        _rightEyeMaterial.SetFloat(Alpha, 0f);
        
        _dying = false;
    }

}
