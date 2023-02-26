using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {
    [SerializeField] private Vector2Int fromTo;
    private Ray _ray;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Node[] nodes;
    private AStar _aStar = new AStar();
    private IHeuristic<float> _heuristic = new AsTheCrowFlies();
    private List<Node> path = new List<Node>();
    
    private void Start() {
        AssignNodeNeighbours();
        GetPath(nodes[fromTo.x], nodes[fromTo.y]);
    }

    private void GetPath(Node from, Node to) {
        Stack<Node> path = _aStar.Path(from, to, _heuristic);

        int numInPath = path.Count;
        for (int i = 0; i < numInPath; i++) {
            this.path.Add(path.Pop());
        }
    }
    
    private void Update() {
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

            //Debug.DrawRay(mySurface, direction, Color.magenta);
                
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
}
