using System;
using UnityEditor;
using UnityEngine;

public class MeshGizmo : MonoBehaviour {
    [SerializeField, HideInInspector] private Mesh mesh;
    [SerializeField, HideInInspector] private Transform _transform;
    [SerializeField] private Color gizmoColor;
    [SerializeField] private Vector3 scale = Vector3.one;
    private Matrix4x4 matrix;

    [SerializeField] private bool hideOnPlay;
    
    private void OnDrawGizmos() {
        if (hideOnPlay && Application.IsPlaying(this))
            return;
        
        mesh ??= GetComponent<MeshFilter>().sharedMesh;
        _transform ??= transform;

        Gizmos.matrix = _transform.localToWorldMatrix;
        
        Gizmos.color = gizmoColor;
        
        Gizmos.DrawMesh(mesh, Vector3.zero, Quaternion.identity, Vector3.one * 1.01f);
    }
    
    void OnDrawGizmosSelected() {
        
        OnDrawGizmosCamera();
        
        Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);

        Gizmos.matrix = _transform.localToWorldMatrix;
        
        Gizmos.DrawMesh(mesh, Vector3.zero, Quaternion.identity, Vector3.one  * 1.01f);
    }

    private void OnDrawGizmosCamera() {
        Camera cam = Camera.main;
        Transform camTransform = cam.transform;
        
        Gizmos.color = Color.white;

        Gizmos.matrix = Matrix4x4.TRS(
            camTransform.position,
            camTransform.rotation,
            camTransform.localScale
        );
        
        Gizmos.DrawFrustum(
            Vector3.zero,
            cam.fieldOfView,
            12.0f,
            cam.nearClipPlane,
            cam.aspect);
    }
}