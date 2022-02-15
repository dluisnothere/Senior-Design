using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Gets all portals in the scene
    Portal[] portals;

    private void Awake()
    {
        portals = FindObjectsOfType<Portal>();
    }

    private void OnPreCull()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].Render();
        }
    }
}
