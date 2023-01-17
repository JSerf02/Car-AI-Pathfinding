using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * General code for a weighted, directed graph
*/
public class Graph : MonoBehaviour
{
    private Dictionary<Vector3, List<Edge>> graph = new Dictionary<Vector3, List<Edge>>();
    public List<Edge> GetEdges(Vector3 point) {
        if(!graph.ContainsKey(point)) {
            return null;
        }
        return graph[point];
    }
    public void AddPoint(Vector3 point) {
        if(graph.ContainsKey(point)) {
            return;
        }
        graph.Add(point, new List<Edge>());
    }
    /*
        Invariant: Each edge in the list of edges must start with the point
    */
    public void AddPoint(Vector3 point, List<Edge> edges) {
        if(graph.ContainsKey(point)) {
            return;
        }
        graph.Add(point, edges);
    }
    public void AddPoint(List<Edge> edges) {
        if(edges.Count == 0) {
            return;
        }
        Vector3 point = edges[0].Start;
        if(graph.ContainsKey(point)) {
            return;
        }
        graph.Add(point, edges);
    }
    public void AddEdge(Vector3 point, Edge edge) {
        if(!graph.ContainsKey(point)) {
            AddPoint(point);
        }
        List<Edge> edges = graph[point];
        foreach(Edge graphEdge in edges) {
            if(graphEdge.End == edge.End) {
                return;
            }
        }
        graph[point].Add(edge);
    }
    public void AddEdge(Edge edge) {
        AddEdge(edge.Start, edge);
    }
    public class Edge {
        private Vector3 _start;
        public Vector3 Start {
            get {
                return _start;
            }
            set {
                _start = value;
            }
        }
        private Vector3 _end;
        public Vector3 End {
            get {
                return _end;
            }
            set {
                _end = value;
            }
        }
        private float _weight;
        public float Weight {
            get {
                return _weight;
            }
            set {
                _weight = value;
            }
        }
        public Edge(Vector3 newStart, Vector3 newEnd, float newWeight) {
            Start = newStart;
            End = newEnd;
            Weight = newWeight;
        }
    }
    

}
