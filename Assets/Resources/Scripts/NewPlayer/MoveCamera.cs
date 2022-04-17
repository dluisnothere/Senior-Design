using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform targetPosition;

    // Update is called once per frame
    // Makes camera always move with player
    void Update()
    {
        this.transform.position = targetPosition.position;
    }
}
