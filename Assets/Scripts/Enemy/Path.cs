using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

public class Path : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    [SerializeField] 
    private bool alwaysDrawPath;
    [SerializeField]
    private bool drawAsLoop;
    [SerializeField]
    private bool drawNumbers;
    public Color debugColour = Color.white;

#if UNITY_EDITOR
    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            GUIStyle labelSryle = new GUIStyle();
            labelSryle.fontSize = 30;
            labelSryle.normal.textColor = debugColour;
            if(drawNumbers)
            {
                Handles.Label(waypoints[i].position, i.ToString(), labelSryle);
            }
            if (i >= 1)
            {
                Gizmos.color = debugColour;
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
                if (drawAsLoop)
                {
                    Gizmos.DrawLine(waypoints[waypoints.Count - i].position, waypoints[0].position);
                }
            }
        }
    }
    public void OndrawGizmosSelected()
    {
        if (alwaysDrawPath)
        {
            return;
        }
        else
        {
            DrawPath();
        }
    }
#endif
}
