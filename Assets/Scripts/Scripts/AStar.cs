using System.Collections.Generic;
using UnityEngine;

public class Comparator<T> : IComparer<Node> {
    private readonly Dictionary<Node, float> _fCost;

    public Comparator(Dictionary<Node, float> fCost) {
        _fCost = fCost;
    }

    public int Compare(Node x, Node y) {
        float xCost = _fCost[x];
        float yCost = _fCost[y];

        return xCost.CompareTo(yCost);
    }
}

public class AStar {
    public Stack<Node> Path(Node start, Node goal,  IHeuristic<float> heuristic) {
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>(100);
        Dictionary<Node, float> gScore = new Dictionary<Node, float>(100);
        Dictionary<Node, float> fScore = new Dictionary<Node, float>(100);
        
        Comparator<float> comparator = new Comparator<float>(fScore);
        Heap<Node> openSet = new Heap<Node>(comparator, 100);
        
        gScore.Add(start, 0);
        fScore.Add(start, heuristic.CostFunction(start, goal));
        openSet.Insert(start);
        
        while (!openSet.Empty()) {
            Node current = openSet.DeleteMin(); 
            
            if (current == goal)
                return Path(cameFrom, current);

            int numNeighbours = current.neighbours.Length;

            for (int n = 0; n < numNeighbours; n++) {
                Node neighbour = current.neighbours[n];
                float tentativeGScore = gScore[current] + neighbour.Cost(current); // make edges have the weight, allows for bi-directionality

                if (!gScore.ContainsKey(neighbour) || tentativeGScore < gScore[neighbour]) {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = tentativeGScore + heuristic.CostFunction(neighbour, goal);

                    openSet.Insert(neighbour);
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

public interface IHeuristic <T>{
    public abstract T CostFunction(Node from, Node to);
}

public class Euclidean : IHeuristic<float> {
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
