using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Initializes a graph that forms a G_1 continuous spline based on a function that
 * ensures constant radial velcity away from each control point
*/
public class CarGraph : Graph
{
    [SerializeField, Tooltip("The radial speed of the car")]
    public float radialSpeed;
    /*
     * In this graph, each node is a pair (x, y) of a point in the world and an
     * angle which can mean one of two things. If the node is the first in an edge,
     * the angle means the angle of the velocity of the car at that point. If the
     * node is the second in an edge, the angle means the angle of the vector
     * endPosition - startPosition, which is the angle the car attempts to rotate
     * towards.
     * - The angle is accessed through edge.z for any given edge in the graph
    */
    public void AddEdge(Vector3 start, Vector2 end) {
        Edge newEdge = NewEdge(start, end);
        AddEdge(start, newEdge);
    }

    /*
     * Add a new edge to the graph with a weight based on the distance it would 
     * take to travel from the start point to the next when moving at a constant
     * radial velocity with linearly extrapolated angular velocity
     * - Modeling the motion of the following parametric polar equation:
     *   - r = kt, theta = theta_0 + k(theta_f - theta_0) / R * t
     *     - r is distance from start point
     *     - theta is angle relative to start point
     *     - theta_0 is starting angle
     *     - theta_f is final angle
     *     - R is the distance between the start and end points
     *     - k is the constant rate of radial velocity
    */
    public static Edge NewEdge(Vector3 start, Vector2 end) {
        
        Vector2 startPoint = new Vector2(start.x, start.y);
        float thetaFinal = Vector2.SignedAngle(Vector2.right, end - startPoint) + 360;
        thetaFinal = thetaFinal % 360;
        float weight = (end - startPoint).magnitude;
        if(thetaFinal != start.z) {
            float deltaTheta = Mathf.Deg2Rad * Mathf.Abs(thetaFinal - start.z);
            weight = weight / 2 * (ASinH(deltaTheta) / deltaTheta + Mathf.Sqrt(deltaTheta * deltaTheta + 1));
        }
        return new Edge(start, new Vector3(end.x, end.y, thetaFinal), weight);
    }

    /*
     * Computes the angle of the velocity vector at the endpoint
     * - Uses the derivative of the equation from NewEdge():
     *   - v(t) = {k*cos(omega * t + z)  + k * omega * t * sin(omega * t + z)}
     *     - omega = k * (theta_f - theta_0) * t / R
    */
    public float GetNextAngle(Edge prevEnd) {
        Vector3 prevTarget = prevEnd.End;
        Vector3 prevBase = prevEnd.Start;
        float distance = Mathf.Sqrt(Mathf.Pow(prevTarget.x - prevBase.x, 2) + Mathf.Pow(prevTarget.y - prevBase.y, 2));
        float time = distance / radialSpeed;
        float prevBaseAngle = prevBase.z * Mathf.Deg2Rad;
        float prevTargetAngle = prevTarget.z * Mathf.Deg2Rad;
        float omegaT = prevTargetAngle - prevBaseAngle; // omega * t when t = R / k
        Vector2 velocity = new Vector2(
            radialSpeed * Mathf.Cos(omegaT + prevBaseAngle) - radialSpeed * omegaT * Mathf.Sin(omegaT + prevBaseAngle),
            radialSpeed * Mathf.Sin(omegaT + prevBaseAngle) + radialSpeed * omegaT * Mathf.Cos(omegaT + prevBaseAngle));
        return (Vector2.SignedAngle(Vector2.right, velocity) + 360) % 360;
    }

    // Gets arcsinh of an angle in radians
    private static float ASinH(float angle) {
        return Mathf.Log(angle + Mathf.Sqrt(Mathf.Pow(angle, 2) + 1));
    }
}
