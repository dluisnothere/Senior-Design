using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyChest : MonoBehaviour
{
    public Portal targetPosition;
    // simply teleports player without portal logic
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("teleport");
        other.gameObject.transform.position = targetPosition.gameObject.transform.position;   
    }
}
