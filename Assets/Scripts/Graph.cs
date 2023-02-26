using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {
    [SerializeField] private Vector2Int fromTo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Node[] nodes;
    [SerializeField] private Transform player;
    
    private AStar _aStar = new AStar();
    private IHeuristic<float> _heuristic = new AsTheCrowFlies();
    private List<Node> path = new List<Node>();

    private Ray _ray;
    
    private void Start() {
        AssignNodeNeighbours();
    }
    
    private void GetPath(Node from, Node to) {
        this.path.Clear();
        Stack<Node> path = _aStar.Path(from, to, _heuristic);

        int numInPath = path.Count;
        for (int i = 0; i < numInPath; i++) {
            this.path.Add(path.Pop());
        }
    }
    
    private void Update() {
        
        _closestNode = FindClosestNode(player.position);
        
        GetPath(_closestNode, nodes[fromTo.y]);
        
        for (int i = 1; i < path.Count; i++) {
            Debug.DrawLine(path[i - 1].position, path[i].position, Color.magenta);
        }
    }

    private void AssignNodeNeighbours() {
        for (int node = 0; node < nodes.Length; node++)
            nodes[node].neighbours = GetNeighbours(nodes[node]).ToArray();
    }

    private List<Node> GetNeighbours(Node node) {
        List<Node> neighbourNodes = new List<Node>();
        
        for (int otherNode = 0; otherNode < nodes.Length; otherNode++) {
            if (node == nodes[otherNode]) continue;
                
            Vector3 myPos = node.position;
            Vector3 other = nodes[otherNode].position;
            Vector3 direction = (other - myPos).normalized;
            Vector3 mySurface = myPos + direction * .5f;
            _ray = new Ray(mySurface, direction);

            if (Physics.Raycast(
                    _ray, 
                    out RaycastHit hitInfo, 
                    float.MaxValue, 
                    layerMask, 
                    QueryTriggerInteraction.Collide)
                && hitInfo.transform == nodes[otherNode].transform) {
                Node neighbourNode = hitInfo.transform.GetComponent<Node>();
                neighbourNodes.Add(neighbourNode);
            }
        }

        return neighbourNodes;
    }

    private Node FindClosestNode(Vector3 position) {
        const float heightToleranceUp = 2f;
        const float heightToleranceDown = 1f;
        
        float closestDistance = float.MaxValue;
        Node closestNode = null;

        for (int node = 0; node < nodes.Length; node++) {
            float distance = Vector3.Distance(nodes[node].position, position);
            if (distance < closestDistance && NotOnOtherFloor(nodes[node])) {
                closestDistance = distance;
                closestNode = nodes[node];
            }
        }

        return closestNode;

        bool NotOnOtherFloor(Node n) {
            float heightAbovePlayer = n.position.y - position.y;
            return heightAbovePlayer < heightToleranceUp && heightAbovePlayer > -heightToleranceDown;
        }
    }

    private Node _closestNode;
    private void OnDrawGizmos() {
        if (_closestNode != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_closestNode.position, .15f);
        }
    }
}
