using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyBox : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Toy"))
        {
            Debug.Log("Thank you for putting it back!");
            Time.timeScale = 0;
        }
    }
}
