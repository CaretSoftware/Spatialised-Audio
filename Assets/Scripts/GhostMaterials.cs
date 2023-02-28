using System;
using UnityEngine;

public class GhostMaterials : MonoBehaviour {
    [SerializeField] private MeshRenderer ghostMeshRenderer;
    [SerializeField] private MeshRenderer leftEyeMeshRenderer;
    [SerializeField] private MeshRenderer rightEyeMeshRenderer;
    
    public Material[] Materials => _materials;
    public Material GhostMaterial => _ghostMaterial;
    public Material LeftEyeMaterial => _leftEyeMaterial;
    public Material RightEyeMaterial => _rightEyeMaterial;

    private Material[] _materials = new Material[3];
    private Material _ghostMaterial;
    private Material _leftEyeMaterial;
    private Material _rightEyeMaterial;


    private void Awake() {
        _materials[0] = _ghostMaterial = ghostMeshRenderer.material;
        _materials[1] = _leftEyeMaterial = leftEyeMeshRenderer.material;
        _materials[2] = _rightEyeMaterial = rightEyeMeshRenderer.material;
    }
}
