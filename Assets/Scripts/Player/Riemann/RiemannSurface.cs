using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiemannSurface : MonoBehaviour
{
    public PlayerMovement player;
    /// <summary>
    /// z = a + bi where a and b dictate how big the radius of this surface is.
    /// z^1/n = will have multiple real values. 
    /// </summary>
    public float a, x; // real components of z and z^1/n... although real x is probably not necessary
    public float b, y; // imaginary component of z
    public float n; // number of levels

    public Vector3 position; // origin of riemann surface

    private void Start()
    {
        // dictates location of Riemann Surface's origin
    }

    private void Update()
    {
        
    }

    // Solve the nth root of the specified complex number a + bi, then floors it
    // Called by player.
    // links portals
    public int Solve(int k)
    {
        // 1. convert to polar coordinates
        // 2. calculate r by getting the player's current position and subtracting with the origin of the surface.
        // 3. calculate angle theta relative to the seam AKA the portal location
        // 4. Iterate over k = {1,2,...,n-1} and solve for the root corresponding to each k using DeMoivre's Theorem
        // 5. Each root [(rel, im) pair] corresponds to a unique room. Just need to know which room you want. 
        
        return 0;
    }
}
