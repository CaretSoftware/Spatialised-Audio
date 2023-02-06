using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DrawCameraFrustrum : MonoBehaviour {
#if UNITY_EDITOR
    
    Camera m_camera;
    private float farClipPlane = 12.0f;

    void OnDrawGizmos() {
        m_camera ??= gameObject.GetComponent<Camera>();
        
        Color tempColor = Gizmos.color;
        Matrix4x4 tempMat = Gizmos.matrix;
        
        if (this.m_camera.orthographic) {
            Camera c = m_camera;
            var size = c.orthographicSize;
            Gizmos.DrawWireCube(Vector3.forward * (c.nearClipPlane + (c.farClipPlane-c.nearClipPlane)/2)
                , new Vector3(size * 2.0f, size * 2.0f * c.aspect, c.farClipPlane-c.nearClipPlane));
        } else {
            Transform t = transform;
            Camera c = m_camera;
            if (farClipPlane <= Mathf.Epsilon) farClipPlane = c.farClipPlane;
            Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
            Gizmos.DrawFrustum(
                Vector3.zero, 
                c.fieldOfView, 
                farClipPlane, 
                c.nearClipPlane, 
                c.aspect);
        }
        Gizmos.color = tempColor;
        Gizmos.matrix = tempMat;
    }
    
#endif
}