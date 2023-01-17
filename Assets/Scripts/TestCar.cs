using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCar : MonoBehaviour
{
    [SerializeField, Tooltip("The car graph")]
    private CarGraph carGraph;
    private Vector3 basePoint;
    private Vector3 targetPoint;
    private float theta;
    private float omega;
    private float timeSoFar = 0;
    private float maxTime;
    private bool started = false;
    private float rotationOffset = 0;
    // Ensure setup is after the graph is setup
    void Start()
    {
        StartCoroutine(LateStart());
    }
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        Setup();
        started = true;
    }

    private void Setup() {
        basePoint = StartPoint.Instance.startPoint;
        SwitchTarget();
    }

    private void SetOmega() {
        // Omega = k * (theta_f - theta_0) / R
        omega = carGraph.radialSpeed * (targetPoint.z - basePoint.z) / Mathf.Sqrt(Mathf.Pow(targetPoint.x - basePoint.x, 2) + Mathf.Pow(targetPoint.y - basePoint.y, 2));
    }

    private void MoveToNextPoint() {
        Graph.Edge prevEdge = carGraph.GetEdges(basePoint)[0];
        basePoint = targetPoint;
        basePoint.z = carGraph.GetNextAngle(prevEdge);
        // Teleport to start point if point isn't found
        if(carGraph.GetEdges(basePoint) == null) {
            basePoint = StartPoint.Instance.startPoint;
        }
        SwitchTarget();
    }

    private void SwitchTarget() {
        rotationOffset = (basePoint.z + transform.eulerAngles.y) % 360;
        theta = basePoint.z;
        targetPoint = carGraph.GetEdges(basePoint)[0].End;
        SetOmega();
        // maxTime = R / k
        maxTime = Mathf.Sqrt(Mathf.Pow(targetPoint.x - basePoint.x, 2) + Mathf.Pow(targetPoint.y - basePoint.y, 2)) / carGraph.radialSpeed;
        timeSoFar = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!started) {
            return;
        }
        Move();
    }

    private void CounterRotate() {
        transform.eulerAngles = Vector3.zero;
        transform.RotateAround(new Vector3(basePoint.x, transform.position.y, basePoint.y), Vector3.up, rotationOffset);
    }

    /* 
     * Moves using polar coordinates by first moving r along the x-axis, then 
     * rotating theta degrees around basePoint
    */
    private void Move() {
        // Update t for future r and theta calculations
        timeSoFar = Mathf.MoveTowards(timeSoFar, maxTime, Time.fixedDeltaTime);

        // Ensure revolution doesn't mess up local rotation
        CounterRotate();

        // Move along x-axis according to radial speed
        transform.position = new Vector3(basePoint.x + carGraph.radialSpeed * timeSoFar, transform.position.y, basePoint.y);
        
        // Rotate around basePoint
        theta = (theta + omega * Time.fixedDeltaTime) % 360;
        transform.RotateAround(new Vector3(basePoint.x, transform.position.y, basePoint.y), Vector3.down, theta); // By the right-hand rule, when x and z are positive, y is negative
        
        // Switch points if the car reaches the destination
        if(timeSoFar == maxTime) {
            MoveToNextPoint();
        }
    }
}
