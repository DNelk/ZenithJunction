using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance = null;
    public List<Waypoint> Waypoints;
    public Waypoint Model;
    public bool NeighborsAssigned = false;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void AssignNeighbors()
    {
        for (int i = 0; i < Waypoints.Count; i++)
        {
            //Next
            if(i != Waypoints.Count-1)
                Waypoints[i].Next = Waypoints[i + 1];
            else
                Waypoints[i].Next = Waypoints[0];

            //Prev
            if(i == 0)
                Waypoints[i].Previous = Waypoints[Waypoints.Count-1];
            else
                Waypoints[i].Previous = Waypoints[i-1];
        }

        NeighborsAssigned = true;
    }
    /*
    private void CreateCurve()
    {
        //Put a marker on all our points in the curve
        for (int i = 0; i < Waypoints.Length; i++)
        {
            float t = (float) i / 100;
            Vector3 points = CalculateBezier(Waypoints[i], t);
        }
    }

    Vector3 CalculateBezier(Waypoint curve, float t)
    {
        //Lerp between a bunch of points and segments to get the bezier between them
        Vector3 a = curve.startPoint;
        Vector3 b = curve.startTangent;
        Vector3 c = curve.endTangent;
        Vector3 d = curve.endPoint;

        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        Vector3 cd = Vector3.Lerp(c, d, t);

        Vector3 abc = Vector3.Lerp(ab, bc, t);
        Vector3 bcd = Vector3.Lerp(bc, cd, t);

        return Vector3.Lerp(abc, bcd, t);
    }*/

    public void RecalculateLinearDist()
    {
        foreach (Waypoint w in Waypoints)
            w.RecalculateLinearDist();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(WaypointManager))]
public class CreateBezier : Editor
{
    //Create a custom GUI that allows us to create new points on our curve
    public override void OnInspectorGUI()
    {
        WaypointManager controller = (WaypointManager) target; //What we're GUI for

        DrawDefaultInspector();

        //Our make curve button
        if (GUILayout.Button("Make Waypoint"))
        {
            Waypoint newPoint = controller.Model.gameObject.AddComponent<Waypoint>(); //Our new curve

            if (controller.Waypoints.Count > 0) //If our curve list isnt empty
            {
                Waypoint lastPoint = controller.Waypoints[controller.Waypoints.Count - 1]; //Our last curve that we need to link to
                
                //Link the curves
                newPoint.startPoint = lastPoint.endPoint;
                newPoint.endPoint = lastPoint.endPoint;
                newPoint.startTangent = lastPoint.endPoint;
                newPoint.endTangent = lastPoint.endPoint;

            }
            newPoint.RecalculateLinearDist();
            controller.Waypoints.Add(newPoint); //Add it to our list
        }
        
        if (GUILayout.Button("Recalculate linear distance"))
            controller.RecalculateLinearDist();
    }
}
#endif