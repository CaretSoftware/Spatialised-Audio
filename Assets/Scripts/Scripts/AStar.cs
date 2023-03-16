using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar {
    
    public Stack<Node> Path(Node start, Node goal,  IHeuristic<float> heuristic) {
        Heap<HeapNode> openSet = new Heap<HeapNode>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>(100);
        Dictionary<Node, float> gScore = new Dictionary<Node, float>(100);
        Dictionary<Node, float> fScore = new Dictionary<Node, float>(100);
        
        gScore.Add(start, 0);
        fScore.Add(start, heuristic.CostFunction(start, goal));

        openSet.Insert(new HeapNode(start, 0));

        while (!openSet.Empty()) {
            Node current = openSet.DeleteMin().node; 
            
            if (current == goal)
                return Path(cameFrom, current);

            int numNeighbours = current.neighbours.Length;

            for (int n = 0; n < numNeighbours; n++) {
                Node neighbour = current.neighbours[n];
                float tentativeGScore = gScore[current] + neighbour.Cost(current); // make edges have the weight, allows for bi-directionality

                if (!gScore.ContainsKey(neighbour) || tentativeGScore < gScore[neighbour]) {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    float f = tentativeGScore + heuristic.CostFunction(neighbour, goal);
                    fScore[neighbour] = f;
                    
                    openSet.Insert(new HeapNode(neighbour, f));   // heap doesn't allow duplicate Nodes
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
    public class HeapNode : IComparable<HeapNode>, IComparable {
        public readonly Node node;
        public float fScore;
        
        public HeapNode(Node node,float fScore) {
            this.node = node;
            this.fScore = fScore;
        }

        public int CompareTo(HeapNode other) {
            return fScore.CompareTo(other.fScore);
        }

        public int CompareTo(object obj) {
            HeapNode other = obj as HeapNode; // avoid double casting
            if (other == null) {
                throw new ArgumentException("A Node object is required for comparison.", nameof(obj));
            }
        
            return CompareTo(other);
        }

        public override bool Equals(object obj) {
            HeapNode other = obj as HeapNode; // avoid double casting
            if (other == null) {
                throw new ArgumentException("A Node object is required for comparison.", nameof(obj));
            }
            return Equals(other);
        }

        protected bool Equals(HeapNode other) {
            return Equals(node, other.node);
        }

        public override int GetHashCode() {
            return (node != null ? node.GetHashCode() : 0);
        }
    }


public class Comparator : IComparer<(float, Node)> {
    public int Compare((float, Node) x, (float, Node) y) {
        if (x.Item1 > y.Item1)
            return 1;
        else
            return -1;
    }
}

public interface IHeuristic <T> {
    public abstract T CostFunction(Node from, Node to);
}

public class AsTheCrowFlies : IHeuristic<float> {
    public float CostFunction(Node from, Node to) {
        return Vector3.Distance(from.position, to.position);
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
