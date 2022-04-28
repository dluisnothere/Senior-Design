using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscherEntrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement p = other.gameObject.GetComponent<PlayerMovement>();
        if (p)
        {
            p.escherPhysicsMode = true;
        }
    }
}
