using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;

public class GhostDeath : MonoBehaviour {
    public delegate void Died();
    public static Died died;

    [SerializeField] private MeshRenderer ghostMeshRenderer;
    [SerializeField] private MeshRenderer leftEyeMeshRenderer;
    [SerializeField] private MeshRenderer rightEyeMeshRenderer;
    [SerializeField] private Rigidbody leftEyeRigidBody;
    [SerializeField] private Rigidbody rightEyeRigidBody;
    [SerializeField] private Collider leftEyeCollider;
    [SerializeField] private Collider rightEyeCollider;
    [SerializeField] private LookAtConstraint lookAtConstraint;
    
    private Transform _playerTransform;
    private Transform _transform;

    private MaterialPropertyBlock _mpb;
    
    private bool _dying;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    private void Die() {
        if (!_dying)
            StartCoroutine(DeathEffect());
    }
    
    private void Awake() {
        died += Die;
        _playerTransform = PlayerTransform.PTransform;
        _transform = transform;
        
        _mpb = new MaterialPropertyBlock();
    }

    private void OnDestroy() {
        died -= Die;
    }

    private void Start() {
        ConstraintSource source = new ConstraintSource {
            sourceTransform = _playerTransform
        };
        lookAtConstraint.AddSource(source);

        _mpb.SetFloat(Alpha, 0f);
        ghostMeshRenderer.SetPropertyBlock(_mpb);
        
        //_eyesMpb.SetFloat(Alpha, 0f);
        leftEyeMeshRenderer.SetPropertyBlock(_mpb);
        rightEyeMeshRenderer.SetPropertyBlock(_mpb);
    }

    private IEnumerator DeathEffect() {
        _dying = true;
        float dropEyeTime = .8f;
        bool droppedEyes = false;
        float t = 0f;
        
        // show ghost
        // TODO animate scale Ease.InElastic/InBack?
        _mpb.SetFloat(Alpha, 1f);
        ghostMeshRenderer.SetPropertyBlock(_mpb);
        leftEyeMeshRenderer.SetPropertyBlock(_mpb);
        rightEyeMeshRenderer.SetPropertyBlock(_mpb);
        
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
                leftEyeRigidBody.isKinematic = false;
                rightEyeRigidBody.isKinematic = false;
                leftEyeRigidBody.AddTorque(UnityEngine.Random.insideUnitSphere); // TODO 
                rightEyeRigidBody.AddTorque(UnityEngine.Random.insideUnitSphere); // TODO 
                
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

            _mpb.SetFloat(Alpha, alpha);
            leftEyeMeshRenderer.SetPropertyBlock(_mpb);
            rightEyeMeshRenderer.SetPropertyBlock(_mpb);
            
            t += Time.deltaTime * (1f / secondsToFade);
            yield return null;
        }
        
        _mpb.SetFloat(Alpha, 0f);
        leftEyeMeshRenderer.SetPropertyBlock(_mpb);
        rightEyeMeshRenderer.SetPropertyBlock(_mpb);
        
        _dying = false;
            
        SpawnManager.respawnGhost?.Invoke();
    }
}
