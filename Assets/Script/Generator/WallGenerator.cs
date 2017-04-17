using UnityEngine;
using System.Collections.Generic;
using System;

public class WallGenerator : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private int numbers = 20;
    [SerializeField]
    Rect rect = new Rect(-100, -50, 200, 100);
    [SerializeField]
    private float min_radius = -.5f;
    [SerializeField]
    private float max_radius = 1f;
    [SerializeField]
    private GameObject wallPrefap;
    [SerializeField]
    private bool discrete = true;
    private PolygonCollider2D polygonCollider;
    private System.Random random = new System.Random();
    private Vector3[] points;
    List<Vector3> blankPosition;
   
    #endregion

    #region Initialization

    void Start()
    {
        blankPosition = new List<Vector3>();
        points = MakeRandomPolygon(numbers, rect);
        BuildTheWall();
        Debug.Log("Length: " + points.Length);
    }
    
    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
    #endregion

    #region Public Members

    public Vector3[] Points
    {
        get
        {
            return points;
        }

        set
        {
            points = value;
        }
    }

    public List<Vector3> BlankPosition
    {
        get
        {
            return blankPosition;
        }

        set
        {
            blankPosition = value;
        }
    }

    #endregion

    #region Private Members

    private void BuildTheWall()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            if(!discrete)
            {
                if(UnityEngine.Random.Range(0,2)<1)
                {
                    Vector3 center = (points[i] + points[i+1]) / 2f;
                    blankPosition.Add(center);
                    continue;
                }
            }
            CreateWall(points[i], points[i + 1]);
        }
        CreateWall(points[points.Length - 1], points[0]); // enclose the wall
    }

    private void CreateWall(Vector3 start, Vector3 end)
    {
        int distance = (int)Vector3.Distance(start, end);
        Vector3 center = (start + end) / 2f;
        center.y = 1;
        Vector3 directionVector = end - start;
        GameObject wall = (GameObject)Instantiate(wallPrefap);
        wall.transform.localPosition = center;
        wall.transform.localRotation = Quaternion.LookRotation(directionVector.normalized);
        wall.transform.localScale = new Vector3(1f, 50f, distance);
        wall.transform.parent = this.transform;
    }

    private Vector3[] MakeRandomPolygon(int numberOfVertices, Rect bounds)
    {
      
        double[] randomRadiusArray = new double[numberOfVertices];  // Pick randomly some radius.
        for (int i = 0; i < numberOfVertices; i++)
        {
            randomRadiusArray[i] = random.NextDouble(min_radius, max_radius);
        }

        double[] angleFractions = new double[numberOfVertices];     // Pick randomly angle fractions.
        const double minFraction = 1.0;
        const double maxFraction = 10.0;
        double totalFraction = 0;
        for (int i = 0; i < numberOfVertices; i++)
        {
            angleFractions[i] = random.NextDouble(minFraction, maxFraction);
            totalFraction += angleFractions[i];
        }

        double[] angles = new double[numberOfVertices];    // Convert the fractions into angle of 2 * Pi radians.
        double to_radians = 2 * Math.PI / totalFraction;
        for (int i = 0; i < numberOfVertices; i++)
        {
            angles[i] = angleFractions[i] * to_radians;
        }
        
        Vector3[] points = new Vector3[numberOfVertices]; // get the locations.
        float radiusX = bounds.width / 2f;
        float radiusY = bounds.height / 2f;
        float centerX = bounds.MidX();
        float centerY = bounds.MidY();
        double theta = 0;
        for (int i = 0; i < numberOfVertices; i++)
        {
            points[i] = new Vector3(
                centerX + (int)(radiusX * randomRadiusArray[i] * Math.Cos(theta)),
                0f,
                centerY + (int)(radiusY * randomRadiusArray[i] * Math.Sin(theta)));
            theta += angles[i];
        }

        // Return the points.
        return points;
    }


    [ContextMenu("Recalculate")]
    void DoSomething()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        points = MakeRandomPolygon(numbers, rect);
        BuildTheWall();
    }
    #endregion
}
