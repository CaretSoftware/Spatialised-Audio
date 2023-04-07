using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadphoneUIMover : MonoBehaviour {

    public delegate void ToggleHeadphones(bool on);
    public static ToggleHeadphones headphonesOff;

    [SerializeField] private Transform headPhones;
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    private void Awake() {
        headphonesOff += HeadPhoneVisuals;
    }

    private void OnDestroy() {
        headphonesOff -= HeadPhoneVisuals;
    }

    private void HeadPhoneVisuals(bool off) {
        Material mat = off ? offMaterial : onMaterial;
        for (int i = 0; i < meshRenderers.Length; i++) {
            Material[] materials = meshRenderers[i].materials;
            for (int j = 0; j < meshRenderers[i].materials.Length; j++) {
                
                materials[j] = mat;
            }
            
            meshRenderers[i].materials = materials;
        }
    }

    private void LateUpdate() {
        Vector3 localPos = headPhones.localPosition;
        Quaternion localRotation = headPhones.localRotation;
        
        transform.SetLocalPositionAndRotation(localPos, localRotation);
    }
}
