using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{   
    private static StartPoint _instance;

    public static StartPoint Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    [SerializeField, Tooltip("The car graph")]
    private CarGraph carGraph;
    [SerializeField, Tooltip("The transform of the tangent object. Initial velocity will point in the direction of the lline formed between this object and the tangent.")]
    private Transform tangentObj;
    [SerializeField, Tooltip("All of the point objects, in order of how they will be initialized")]
    private List<Point> points;
    
    // The starting point for the graph, initialized in start
    [System.NonSerialized]
    public Vector3 startPoint;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize startPoint with the angle of the tangent line between this object and tangentObj
        startPoint = new Vector3(transform.position.x, transform.position.z, 
            (Vector2.SignedAngle(Vector2.right, new Vector2(tangentObj.position.x - transform.position.x, tangentObj.position.z - transform.position.z)) + 360) % 360);
        
        // Iterate through all of the points in order and add them
        Vector3 cur = startPoint;
        foreach(Point point in points) {
            Vector3 pointPosition = point.transform.position;
            Vector2 pointPosition2D = new Vector2(pointPosition.x, pointPosition.z);
            carGraph.AddEdge(cur, pointPosition2D);
            Graph.Edge prevEdge = CarGraph.NewEdge(cur, pointPosition2D);
            cur = prevEdge.End;
            cur.z = carGraph.GetNextAngle(prevEdge); // Start angles are different from end angles. See CarGraph.cs
        }
        carGraph.AddEdge(cur, new Vector2(startPoint.x, startPoint.y)); // Go back to the start at the end
        // Returning to the start will likely not be C_1 continuous
    }
}
