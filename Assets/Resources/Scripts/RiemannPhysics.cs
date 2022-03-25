using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiemannPhysics : MonoBehaviour
{
    /// <summary>
    /// z = a + bi where a and b dictate how big the radius of this surface is.
    /// z^1/n = will have multiple real values. 
    /// </summary>
    public float radius;
    public float totalTheta; // angle around 
    public float n; // number of levels total
    public int currentLevelK;

    public GameObject originPole; // origin of riemann surface
    public Vector3 seamAxis;
    public Vector3 origin;
    public Vector3 prevPosition;

    public Vector3 localUp;

    //public Rigidbody rb;
    //public float speed;

    //public bool pickedUp;

    public GlobalScript globalObj;

    //private void Start()
    //{
    //    // dictates location of Riemann Surface's origin
    //    currentPos = this.transform.position;
    //    origin = new Vector3(this.originPole.transform.position.x, currentPos.y, this.originPole.transform.position.z);
    //    radius = Vector3.Magnitude(currentPos - origin);
    //    //Debug.Log("radius: " + radius);
    //    theta = 0;
    //    n = 3;

    //    pickedUp = false;

    //    rb = this.gameObject.GetComponent<Rigidbody>();

    //    if (origin.y > 0)
    //    {
    //        Debug.Log(origin.y);
    //        currentLevelK = 0;
    //    }
    //    if (origin.y >= 14)
    //    {
    //        currentLevelK = 1;
    //    } 
    //    if (origin.y > 26)
    //    {
    //        currentLevelK = 2;
    //    }
    //}

    //private void Update()
    //{
    //    //continuously increase angle and solve for ball height
    //    //moves the ball in the direction based on solve.
    //    // Increase angle incrementally but wrap back to 0, when ManualAscend is called, increase theta by 360.
    //    // Using transform.position means rigidbody doesn't really work. Need to move according to rigidbody. 

    //    if (!pickedUp)
    //    {
    //        float x = radius * (float)Math.Cos(theta);
    //        float z = radius * (float)Math.Sin(theta);
    //        float y = (float)Solve(this.currentLevelK);

    //        Vector3 nextPosition = new Vector3(x, y, z) + origin;

    //        Vector3 velDirection = nextPosition - this.transform.position;

    //        this.rb.velocity = velDirection;

    //        float twopi = 2 * Mathf.PI;

    //        if (currentLevelK % 2 == 0)
    //        {
    //            theta += 0.001f;

    //            if (theta > currentLevelK * twopi + twopi)
    //            {
    //                theta = currentLevelK * twopi;
    //            }
    //        } else
    //        {
    //            theta -= 0.001f;
    //            if (theta < currentLevelK * twopi - twopi)
    //            {
    //                theta = currentLevelK * twopi;
    //            }
    //        }
    //    } else
    //    {
    //        this.rb.velocity = new Vector3(0.0f,0.0f,0.0f);
    //    }
    //}

    //public void SetPickedUp(bool status)
    //{
    //    this.pickedUp = status;
    //}

    //public void ManualAscend()
    //{
    //    Debug.Log("Ascend");
    //    this.currentLevelK += 1;
    //    //theta += 2 * Mathf.PI;
    //}

    //public void ManualDescend()
    //{
    //    Debug.Log("Descend");
    //    this.currentLevelK -= 1;
    //    //theta -= 2 * Mathf.PI;
    //}

    //// Solve the nth root of the specified complex number a + bi, then floors it
    //// Called by player.
    //// links portals

    private void Start()
    {
        this.currentLevelK = 0;
        this.radius = 20;
        this.n = 3;
        this.prevPosition = this.transform.position;
        this.origin = this.originPole.transform.position;
        this.globalObj = GameObject.Find("GlobalObject").GetComponent<GlobalScript>() ;
        this.seamAxis = new Vector3(0, 0, -1);
        this.localUp = new Vector3(0, 1, 0); // y axis
        this.totalTheta = Vector3.SignedAngle(this.prevPosition - this.origin, this.seamAxis, this.localUp);

        this.globalObj.EnableObjects(this.currentLevelK);

    }

    private float convertToRadians(float degree)
    {
        return degree * Mathf.PI / 180.0f;
    }

    private void Update()
    {
        float deltaTheta = convertToRadians(Vector3.SignedAngle(this.gameObject.transform.position - this.origin, this.prevPosition - this.origin, this.localUp));
        this.totalTheta += deltaTheta;

        //if (this.totalTheta > currentLevelK * 2.0 * Mathf.PI + 2.0 * Mathf.PI)
        //{
        //    this.currentLevelK++;
        //} else if (totalTheta < currentLevelK * 2.0 * Mathf.PI - 2.0 * Mathf.PI) {

        //    this.currentLevelK--;
        //}
        var calcK = (int)(Math.Floor(this.Solve(0)) + n) / (int)(n - 1);
        if (calcK != this.currentLevelK)
        {
            Debug.Log("About to enable");
            this.currentLevelK = calcK;
            this.globalObj.EnableObjects(this.currentLevelK);
        }
        Debug.Log("currentLevelK: " + this.currentLevelK);
        this.prevPosition = this.gameObject.transform.position;
    }
    public double Solve(int k)
    {
        // 1. convert to polar coordinates
        // 2. calculate r by getting the player's current position and subtracting with the origin of the surface.
        // 3. calculate angle theta relative to the seam AKA the portal location
        // 4. Iterate over k = {1,2,...,n-1} and solve for the root corresponding to each k using DeMoivre's Theorem
        // 5. Each root [(rel, im) pair] corresponds to a unique room. Just need to know which room you want. 
        double rRootN = Math.Pow(radius, 1 / n);
        double a = Math.Cos((this.totalTheta + 2*Mathf.PI) / n);
        double b = Math.Sin((this.totalTheta + 2*Mathf.PI) / n);

        return rRootN * a;
    }
}
