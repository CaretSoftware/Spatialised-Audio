using System;
using UnityEngine;

public class PlayerTransform : MonoBehaviour {
    public static Transform PTransform => _transform;

    private static Transform _transform;
    
    private void Awake() {
        _transform = transform;
    }
}
