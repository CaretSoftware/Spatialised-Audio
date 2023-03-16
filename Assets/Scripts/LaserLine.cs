using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LaserLine : MonoBehaviour {
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    
    public delegate void ShowLineRenderer();
    public static ShowLineRenderer showLineRenderer;

    [FormerlySerializedAs("shootPosition")] [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask layerMask;
    [SerializeField, Range(0f, 6f)] private float laserFadeSpeed = 1f;
    [SerializeField] private Light muzzleFlash;
    [SerializeField] private float muzzleFlashIntensity;
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistanceLaser;
    [SerializeField] private float laserSpeed;

    private Vector3 _screenCenter = new Vector3(.5f, .5f, 0f);
    private MaterialPropertyBlock _mpb;
    private Material _material;
    private float _alpha;
    private float lineWidthStart;
    private float lineWidthEnd;
    private Vector3 laserPrimaryStart;
    private Vector3 laserStart;
    private Vector3 laserEnd;
    private float shotDistance;
    private float shotTravel = -1f;
    private void Awake() {
        _material = lineRenderer.material;
        lineRenderer.material = _material;
        _mpb = new MaterialPropertyBlock();
        showLineRenderer += ShowLaser;
        lineWidthEnd = lineRenderer.endWidth;
        lineWidthStart = lineRenderer.startWidth;

    }

    private void Start() {
        maxDistanceLaser = (firePoint.position - player.position).magnitude;
    }

    private void OnDestroy() {
        showLineRenderer -= ShowLaser;
    }

    private void Update() {

        if (shotTravel >= 0f) {
            shotTravel -= Time.deltaTime * laserSpeed;

            laserStart = Vector3.Lerp(laserPrimaryStart, laserEnd, Ease.InSine( 1f - (shotTravel / shotDistance)));
            lineRenderer.SetPosition(0, laserStart);
            muzzleFlash.transform.position = laserStart;
        }
        
        if (_alpha >= 0f) {
            _alpha -= Time.deltaTime * laserFadeSpeed;
            _alpha = Mathf.Max(0f, _alpha);
            
            _mpb.SetFloat(Alpha, _alpha);
            lineRenderer.SetPropertyBlock(_mpb);
            SetLineWidth(_alpha);

            
            // float laserDistance = (laserStart - player.position).magnitude;

            //if (laserDistance < maxDistanceLaser) {
            //    float diff = maxDistanceLaser - laserDistance;
            //    Vector3 lineDirection = (laserEnd - laserStart).normalized;

            //    laserStart = laserStart + lineDirection * diff;
            //    lineRenderer.SetPosition(0, laserStart);

            //    Debug.Log(diff);
            //}


        }
    }

    private void ShowLaser() {
        _alpha = 1.5f;
        Ray ray = cam.ViewportPointToRay(_screenCenter);

        Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layerMask);
        laserPrimaryStart = firePoint.position;
        laserStart = laserPrimaryStart;
        laserEnd = hitInfo.point;
        lineRenderer.SetPosition(0, laserStart);
        lineRenderer.SetPosition(1, laserEnd);
        shotDistance = (laserEnd - laserStart).magnitude;
        shotTravel = shotDistance;
    }

    private void SetLineWidth(float t) {
        float ease = Ease.OutBounce(t);
        float s = Mathf.LerpUnclamped(0f, lineWidthStart, ease);
        float e = Mathf.LerpUnclamped(0f, lineWidthEnd, ease);
        lineRenderer.endWidth = s;
        lineRenderer.startWidth = e;
        muzzleFlash.intensity = Mathf.LerpUnclamped(0f, muzzleFlashIntensity, ease);
    }
}
