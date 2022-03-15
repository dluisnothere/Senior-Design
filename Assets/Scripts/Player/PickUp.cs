using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform destination;

    void OnMouseDown()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        this.transform.position = destination.position;
        this.transform.parent = GameObject.Find("Destination").transform;

    }

    void OnMouseUp()
    {
        this.transform.parent = null;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.freezeRotation = false;
    }
}
