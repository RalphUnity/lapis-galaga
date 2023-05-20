using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color pathColor = Color.green;

    Transform[] objArray;
    public bool visualizePath;
    [Range(1, 20)] public int lineDensity = 1;
    int overload;

    public List<Transform> pathObjList = new List<Transform>();
    public List<Vector3> bezierObjList = new List<Vector3>();

    void Start()
    {
        CreatePath();
    }

    void OnDrawGizmos()
    {
        if (visualizePath)
        {
            // Straight path
            Gizmos.color = pathColor;
            // Fill the array
            objArray = GetComponentsInChildren<Transform>();
            // ClearObj
            pathObjList.Clear();
            // all children into list
            foreach (Transform obj in objArray)
            {
                if (obj != this.transform)
                {
                    pathObjList.Add(obj);
                }
            }

            // Draw the objects
            for (int i = 0; i < pathObjList.Count; i++)
            {
                Vector3 position = pathObjList[i].position;
                if (i > 0)
                {
                    Vector3 previous = pathObjList[i - 1].position;
                    Gizmos.DrawLine(previous, position);
                    Gizmos.DrawWireSphere(position, 0.3f);
                }
            }

            // Curved path

            // Check overload
            if (pathObjList.Count % 2 == 0)
            {
                pathObjList.Add(pathObjList[pathObjList.Count - 1]);
                overload = 2;
            }
            else
            {
                pathObjList.Add(pathObjList[pathObjList.Count - 1]);
                pathObjList.Add(pathObjList[pathObjList.Count - 1]);
                overload = 3;
            }

            // Curve creation
            bezierObjList.Clear();

            Vector3 lineStart = pathObjList[0].position;

            for (int i = 0; i < pathObjList.Count - overload; i += 2)
            {
                for (int j = 0; j <= lineDensity; j++)
                {
                    Vector3 lineEnd = GetPoint(pathObjList[i].position,
                                               pathObjList[i + 1].position,
                                               pathObjList[i + 2].position,
                                               j / (float)lineDensity);

                    // To visualize the curve
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(lineStart, lineEnd);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(lineStart, 0.1f);

                    lineStart = lineEnd;

                    bezierObjList.Add(lineStart);
                }
            }
        }
        else
        {
            //pathObjList.Clear();
            //bezierObjList.Clear();
        }
    }

    ///<summary>Request a point on a quadratic bezier curve between 3 points</summary>
    Vector3 GetPoint(Vector3 point0, Vector3 point1, Vector3 point2, float timing)
    {
        return Vector3.Lerp(Vector3.Lerp(point0, point1, timing), Vector3.Lerp(point1, point2, timing), timing);
    }

    void CreatePath()
    {
        // Fill the array
        objArray = GetComponentsInChildren<Transform>();
        // ClearObj
        pathObjList.Clear();
        // all children into list
        foreach (Transform obj in objArray)
        {
            if (obj != this.transform)
            {
                pathObjList.Add(obj);
            }
        }

        // Curved path
        // Check overload
        if (pathObjList.Count % 2 == 0)
        {
            pathObjList.Add(pathObjList[pathObjList.Count - 1]);
            overload = 2;
        }
        else
        {
            pathObjList.Add(pathObjList[pathObjList.Count - 1]);
            pathObjList.Add(pathObjList[pathObjList.Count - 1]);
            overload = 3;
        }

        // Curve creation
        bezierObjList.Clear();

        Vector3 lineStart = pathObjList[0].position;

        for (int i = 0; i < pathObjList.Count - overload; i += 2)
        {
            for (int j = 0; j <= lineDensity; j++)
            {
                Vector3 lineEnd = GetPoint(pathObjList[i].position,
                                           pathObjList[i + 1].position,
                                           pathObjList[i + 2].position,
                                           j / (float)lineDensity);
                lineStart = lineEnd;
                bezierObjList.Add(lineStart);
            }
        }
    }
}
