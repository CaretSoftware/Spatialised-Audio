using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IComparable<Node>, IComparable {
    public Node[] neighbours;
    public Vector3 position;
    public static Node goalNode;
    public static IHeuristic<float> heuristic;
    public float fScore = float.MaxValue;

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
    
    public float Cost() {
        return heuristic.CostFunction(this, goalNode);
    }

    public float Cost(Node other) {
        return Vector3.Distance(position, other.position);
    }

    public int CompareTo(object obj) {
        Node other = obj as Node; // avoid double casting
        if (other == null) {
            throw new ArgumentException("A Node object is required for comparison.", nameof(obj));
        }
        
        return CompareTo(other);
    }

    public int CompareTo(Node obj) {
        return obj.fScore.CompareTo(this.fScore);
    }

    private void OnDrawGizmosSelected() {
        if (Application.IsPlaying(this) && neighbours != null)
            DrawDebugRay();
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
