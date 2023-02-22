using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar {
    
    public Stack<Node> Path(Node start, Node goal,  IHeuristic<float> heuristic) {
        Heap<Node> openSet = new Heap<Node>(100);
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>(100);
        Dictionary<Node, float> gScore = new Dictionary<Node, float>(100);
        Dictionary<Node, float> fScore = new Dictionary<Node, float>(100);
        gScore.Add(start, 0);
        fScore.Add(start, heuristic.CostFunction(start, goal));

        openSet.Insert(start);

        while (!openSet.Empty()) {
            Node current = openSet.DeleteMin(); // ?

            if (current == goal)
                return Path(cameFrom, current);

            int numNeighbors = current.neighbors.Length;

            for (int n = 0; n < numNeighbors; n++) {
                Node neighbor = current.neighbors[n];
                float tentativeGScore = gScore[current] + neighbor.Cost(); // make edges have the weight, allows for bi-directionality
                if (tentativeGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current; // risk of duplicates? check if already in Dict?
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + heuristic.CostFunction(current, neighbor);
                    
                    openSet.Insert(neighbor);   // heap doesn't allow duplicates
                }
            }
        }

        return null; // failure
    }

    private Stack<Node> Path(Dictionary<Node, Node> cameFrom, Node current) {
        Stack<Node> totalPath = new Stack<Node>();
        totalPath.Push(current);
        
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            totalPath.Push(current);
        }

        return totalPath;
    }
}

public class Node : IComparable<Node>, IComparable {
    public Node[] neighbors;
    private float cost;
    public Vector3 position;
    
    public Node(int cost) { this.cost = cost; }

    public Node(int cost, Vector3 position, Node[] neighbors) {
        this.cost = cost;
        this.position = position;
        this.neighbors = neighbors;
    }
    
    public float Cost() {
        return cost;
    }

    public void SetCost(float cost) {
        this.cost = cost;
    }
    
    public int CompareTo(Node obj) {
        return obj.cost.CompareTo(this.cost);
    }

    public int CompareTo(object obj) {
        throw new NotImplementedException();
    }
}

public class AsTheCrowFlies : IHeuristic<float> {
    public float CostFunction(Node from, Node to) {
        return (to.position - from.position).magnitude;
    }
}

public class ManhattanXY : IHeuristic<int> {
    public int CostFunction(Node from, Node to) {
        return Mathf.RoundToInt(Mathf.Abs(to.position.x - from.position.x) + Mathf.Abs(to.position.y - from.position.y));
    }
}

public class ManhattanXZ : IHeuristic<int> {
    public int CostFunction(Node from, Node to) {
        return Mathf.RoundToInt(Mathf.Abs(to.position.x - from.position.x) + Mathf.Abs(to.position.z - from.position.z));
    }
}

public class ManhattanXYZ : IHeuristic<int> {
    public int CostFunction(Node from, Node to) {
        return Mathf.RoundToInt(Mathf.Abs(to.position.x - from.position.x) + Mathf.Abs(to.position.y - from.position.y) + Mathf.Abs(to.position.z - from.position.z));
    }
}

public interface IHeuristic <T> {
    public abstract T CostFunction(Node from, Node to);
}

