using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class Graph : MonoBehaviour {
    [SerializeField] private Vector2Int fromTo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Node[] nodes;
    [FormerlySerializedAs("spawnVolumes")] [SerializeField] private Node[] closestNodes;
    [SerializeField] private Transform player;

    [SerializeField, Range(1f, 50f)] private float maxSpawnDistance;
    [SerializeField, Range(1f, 50f)] private float minSpawnDistance;

    private AStar _aStar = new AStar();
    private IHeuristic<float> _heuristic = new AsTheCrowFlies();
    private List<Node> path = new List<Node>();
    
    private SpawnManager _spawnManager;
    private Dictionary<Transform, Node> spawnVolumeClosestNode = new Dictionary<Transform, Node>();
    
    private Ray _ray;

    private void Start() {
        AssignNodeNeighbours();
        _spawnManager = FindObjectOfType<SpawnManager>();
        InitialiseDictionary();
    }

    private void InitialiseDictionary() {
        for (int i = 0; i < _spawnManager.spawnVolumes.Length; i++) {
            Transform spawnVolume = _spawnManager.spawnVolumes[i];
            spawnVolumeClosestNode[spawnVolume] = closestNodes[i];
        }
    }
    
    // array of nodes (same order as association to spawn volume)
    // dictionary<Transform spawnpoint, int index of nodes>

    private Transform last;
    private Transform secondLast;

    public void SpawnGhost() {
        Transform spawnVolume = null;
        Vector3 spawnPosition;
        float distanceToSpawn = float.MaxValue;
        float distanceRelax = 0f;
        Node closestNode = FindClosestNode(player.position);;
        int x = 0;
        
        do {
            if (x % 10 == 0)
                distanceRelax += 5f;
            
            distanceToSpawn = 0f;
            spawnVolume = _spawnManager.RandomSpawnPosition(out spawnPosition);
            // get path to spawnVolume
            Stack<Node> pth = GetPath(closestNode, spawnVolumeClosestNode[spawnVolume]);
            Vector3 pos = player.position;
            int count = pth.Count;
            for (int i = 0; i < count; i++) {
                Vector3 nextPos = pth.Pop().position;
                distanceToSpawn += Vector3.Distance(pos, nextPos);
                pos = nextPos;
            }
            
        } while (   (spawnVolume == secondLast
                  || spawnVolume == last 
                  || distanceToSpawn < minSpawnDistance 
                  || distanceToSpawn > maxSpawnDistance + distanceRelax) 
                    && x++ < 1000);

        if (spawnVolume != null && secondLast != null)
            Debug.Log($"{spawnVolume.name} {last.name} {secondLast.name} distanceToSpawn {distanceToSpawn} min: {minSpawnDistance} max: {maxSpawnDistance} x {x}");
        
        secondLast = last;
        last = spawnVolume;

        if (x >= 999)
            Debug.Log($"PROBABLY WORRY");

        // TODO ghost.position = spawnPosition;
    }
    
    private Stack<Node> GetPath(Node from, Node to) {
        this.path.Clear();
        Stack<Node> path = _aStar.Path(from, to, _heuristic);

        //int numInPath = path.Count;
        //for (int i = 0; i < numInPath; i++) {
        //    this.path.Add(path.Pop());
        //}

        return path;
    }

    public bool debug;
    private void Update() {

        if (debug) {
            debug = false;
            SpawnGhost();
        }
        
        // _closestNode = FindClosestNode(player.position);
        // 
        // GetPath(_closestNode, nodes[fromTo.y]);
        
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
            Vector3 mySurface = myPos + direction * transform.localScale.x * .5f;
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
