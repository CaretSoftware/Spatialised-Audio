using UnityEngine;

public class PositionWithinCube : MonoBehaviour {

    [SerializeField] private float radius = .5f;
    [SerializeField] private LayerMask layerMask;
    
    public Vector3 PositionWithin(Transform cube) {
        Vector3 scale = cube.localScale;
        Vector3 position = cube.position;

        float xMin = position.x - scale.x * .5f + radius;
        float xMax = position.x + scale.x * .5f - radius;
        
        float yMin = position.y - scale.y * .5f + radius;
        float yMax = position.y + scale.y * .5f - radius;
        
        float zMin = position.z - scale.z * .5f + radius;
        float zMax = position.z + scale.z * .5f - radius;

        Vector3 spawnPos = Vector3.zero;

        do {
            spawnPos.x = Random.Range(xMin, xMax);
            spawnPos.y = Random.Range(yMin, yMax);
            spawnPos.z = Random.Range(zMin, zMax);
        } while (Physics.CheckSphere(spawnPos, radius, layerMask, QueryTriggerInteraction.Ignore));

        return spawnPos;
    }
}
