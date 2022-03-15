using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    /// <summary>
    /// z = a + bi where a and b dictate how big the radius of this surface is.
    /// z^1/n = will have multiple real values. 
    /// </summary>
    public int radius;
    public float theta;
    public float n; // number of levels total
    public int currentLevelK;

    public Vector3 origin; // origin of riemann surface
    public Vector3 currentPos;

    public Rigidbody rb;
    public float speed;

    private void Start()
    {
        // dictates location of Riemann Surface's origin
        origin = this.transform.position;
        currentPos = this.transform.position;
        currentLevelK = 0;
        radius = 10;
        theta = 0;
        n = 3;
        speed = 2;

        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //continuously increase angle and solve for ball height
        //moves the ball in the direction based on solve.
        // Increase angle incrementally but wrap back to 0, when ManualAscend is called, increase theta by 360.
        // Using transform.position means rigidbody doesn't really work. Need to move according to rigidbody. 
        
        theta += 0.01f;

        float x = radius * (float)Math.Cos(theta);
        float z = radius * (float)Math.Sin(theta);
        float y = (float)Solve(this.currentLevelK);

        Vector3 nextPosition = new Vector3(x, y, z) + origin;

        Vector3 velDirection = nextPosition - this.transform.position;

        this.rb.velocity = speed * velDirection;

        float twopi = 2*Mathf.PI;

        if (theta > currentLevelK * twopi + twopi)
        {
            theta = currentLevelK * twopi;
        }
        // Debug.Log("theta: " + theta);
    }

    public void ManualAscend(int level)
    {
        this.currentLevelK += 1;
        theta += 2 * Mathf.PI;
    }

    public void ManualDescend(int level)
    {
        this.currentLevelK -= 1;
        theta -= 2 * Mathf.PI;
    }

    // Solve the nth root of the specified complex number a + bi, then floors it
    // Called by player.
    // links portals
    public double Solve(int k)
    {
        // 1. convert to polar coordinates
        // 2. calculate r by getting the player's current position and subtracting with the origin of the surface.
        // 3. calculate angle theta relative to the seam AKA the portal location
        // 4. Iterate over k = {1,2,...,n-1} and solve for the root corresponding to each k using DeMoivre's Theorem
        // 5. Each root [(rel, im) pair] corresponds to a unique room. Just need to know which room you want. 
        double rRootN = Math.Pow(radius, 1 / n);
        double a = Math.Cos((theta + 2*Mathf.PI * k) / n);
        double b = Math.Sin((theta + 2*Mathf.PI * k) / n);

        return rRootN * a;
    }
}
