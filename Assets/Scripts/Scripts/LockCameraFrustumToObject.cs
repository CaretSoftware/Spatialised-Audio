using UnityEngine;
using UnityEngine.Serialization;

//[ExecuteInEditMode]
public class LockCameraFrustumToObject : MonoBehaviour {
    public Camera cam;
    public Transform targetObject;
    public float lensShiftScale = 1.0f;
    
    private void Update() {
        if (targetObject == null) return;
        
        // Calculate the position of the object in screen coordinates
        Vector3 objectScreenPosition = cam.WorldToScreenPoint(targetObject.position);


        // Calculate the normalized lens shift values based on the position of the object
        float lensShiftXNormalized = (objectScreenPosition.x / Screen.width) - 0.5f;

        float lensShiftYNormalized = (objectScreenPosition.y / Screen.height) - 0.5f;
        
        // Apply the normalized lens shift values to the cam
        cam.lensShift = new Vector2(lensShiftXNormalized * lensShiftScale, lensShiftYNormalized * lensShiftScale);
    }
}