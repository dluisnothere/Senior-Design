using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
    public Transform doorDestination;
    public void OnTriggerEnter(Collider other)
    {
        PlayerMovement p = other.gameObject.GetComponent<PlayerMovement>();
        if (p)
        {
            p.transform.position = doorDestination.position;
            p.ResetRotation();
        }
    }
}
