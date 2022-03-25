using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalScript : MonoBehaviour
{
    public void DisableObjects(int levelK)
    {
        Debug.Log("Disable Objects on Level" + levelK);
        GameObject[] objectsOnSurfaceK = GameObject.FindGameObjectsWithTag("Level"+levelK);
        for (int i = 0; i < objectsOnSurfaceK.Length; i++) {
            GameObject obj = objectsOnSurfaceK[i];
            MeshCollider collider = obj.GetComponent<MeshCollider>();
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            collider.enabled = false;
            renderer.material = Resources.Load<Material>("Sprites/Materials/transparent");
        }
    }

    public void EnableObjects(int levelK)
    {
        Debug.Log("Enable Objects on Level" + levelK);
        GameObject[] objectsOnSurfaceK = GameObject.FindGameObjectsWithTag("Level" + levelK);
        for (int i = 0; i < objectsOnSurfaceK.Length; i++)
        {
            Debug.Log("enabling object...");
            GameObject obj = objectsOnSurfaceK[i];
            MeshCollider collider = obj.GetComponent<MeshCollider>();
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            collider.enabled = true;
            renderer.material = Resources.Load<Material>("Sprites/Materials/Default");
        }

        if (levelK == 2)
        {
            this.DisableObjects(1);
            this.DisableObjects(0);
        }

        else if (levelK == 1)
        {
            this.DisableObjects(0);
            this.DisableObjects(2);
        }

        else if (levelK == 0)
        {
            this.DisableObjects(1);
            this.DisableObjects(2);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
