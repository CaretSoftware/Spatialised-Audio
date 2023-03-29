using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    public Node[] neighbours;
    public Vector3 position;
    
    private void Awake() {
        position = transform.position;
    }

    private void DrawDebugRay() {
        for (int node = 0; node < neighbours.Length; node++) {
            for (int neighbour = 0; neighbour < neighbours.Length; neighbour++) {
                Debug.DrawLine(position + Vector3.down * .2f, neighbours[neighbour].position, Color.red);
            }
        }
    }

    public float Cost(Node other) {
        return Vector3.Distance(position, other.position);
    }

    private void OnDrawGizmosSelected() {
        if (Application.IsPlaying(this) && neighbours != null)
            DrawDebugRay();
    }
}
