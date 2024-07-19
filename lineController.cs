using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class lineConroller : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;


    private List<Vector3> DirectionVectors = new List<Vector3>();//List of drection vectors
    private List<Double> angles = new List<Double>();//list of all the angles in the drawing
    private float dotProduct;//the dotproduct of two direction vectors
    private Vector3 crossProduct;//the crossproduct of two direction vectors
    private float mod;//moduluses of two direction vectors timesed together
    private float ang;//the final angle after calculation is performed

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void setUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;// gets al the points
    }

    public bool isCoplanar()
    {
        if (points.Length == 3)//check if there are 3 points then it must be coplanar
        {
            return true;
        }
        else if (points.Length < 3)// if less than 3 points then it cant be a plane at all so isnt coplanar
        {
            return false;
        }
        //first find the equation of a plane with three of the points in form Ax + By + Cz + D = 0
        //formula can also be written as ax + by + cz + (-a*p1 - b*p2 - c*p3) where (a, b, c) are the result from the cross product and (p1, p2, p3) is one of the points used to get the two directionvectors
        //the tollerance is a multiplyer timesd by (a + b + c) and that determines how far away from 0 the solution can be. if the solution to the equation is 0 then they all lie perfectly on one plane, else if the point is off by 0.1 m in all directions from the origional plane it will still be allowed as thast withn the tollerance values. 

        //get two direction vectors of evenly spaced out points across the line, so with 6 points it would get the first, third and fith point as they all have a space of one between them, and add them to pointlist
        DirectionVectors.Add(points[2].position - points[0].position);
        DirectionVectors.Add(points[4].position - points[0].position);

        //finds the cross product of the direction vector referanced as (a, b, c)
        crossProduct = Vector3.Cross(DirectionVectors[0], DirectionVectors[1]);

        //find the value for D
        float D = (-crossProduct.x * points[0].position.x - crossProduct.y * points[0].position.y - crossProduct.z * points[0].position.z);
        //calculate Tollarance(margin of acceptance)
        double tollerance = Math.Round(0.4 * (crossProduct.x + crossProduct.y + crossProduct.z), 5);// the 0.1 multiplyer gives you a margine of 10 cenermetres above or below the plane of the three origional points
        Debug.Log(tollerance);
        //check if the rest of the points lie within the tollerance of the origional plane
        for (int i = 0; i < points.Length; i++)
        {
            double sum = Math.Round(points[i].position.x * crossProduct.x + points[i].position.y * crossProduct.y + points[i].position.z * crossProduct.z, 5);
            Debug.Log(sum);
            Debug.Log(sum < -tollerance);
            Debug.Log(sum > tollerance);
            if (sum < -tollerance || sum > tollerance)
            {
                return false;
            }
        }

        return true;
    }
    private void Update()
    {
        for(int i = 0; i< points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //circular sliding window approach
            //you need 3 points to create two ditection vectors between them so you make a sliding window of size three to go through all combinations of 3 ajacent points
            for (int i = 0; i < 5; i++)
            {
                //calculate the two direction vectors going from the centre point to the two ajacent points
                DirectionVectors.Add(points[0 + i].position - points[1 + i].position);
                DirectionVectors.Add(points[2 + i].position - points[1 + i].position);
                //calculate the dotproduct of those two vectors and the product of the modulus of each of them
                dotProduct = Vector3.Dot(DirectionVectors[0], DirectionVectors[1]);
                mod = DirectionVectors[0].magnitude * DirectionVectors[1].magnitude;
                //apply the values to the formula theta = cos-1 ( (A.B) / (|A||B|) ) to calculate the angle between the vectors
                ang = Mathf.Acos(dotProduct / mod);
                angles.Add(ang / (2 * 3.14) * 360);


                DirectionVectors.Clear();

            }

            DirectionVectors.Add(points[1].position - points[0].position);
            DirectionVectors.Add(points[5].position - points[0].position);
            dotProduct = Vector3.Dot(DirectionVectors[0], DirectionVectors[1]);
            mod = DirectionVectors[0].magnitude * DirectionVectors[1].magnitude;
            ang = Mathf.Acos(dotProduct / mod);
            angles.Add(ang / (2 * 3.14) * 360);


            DirectionVectors.Clear();

            Debug.Log(string.Join(", ", angles));
            if (isCoplanar())
            {
                Debug.Log("the points are all roughly coplainar");
            }
            else
                Debug.Log("your points are all over the place!");
        }
    }
}
