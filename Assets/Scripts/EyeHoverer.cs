using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that holds the eyes' ability to select objects through raycasts
public class EyeHoverer : MonoBehaviour
{
    public bool hitSomething = false;
    public GameObject hitObject;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
        {
            Hit(hit.transform.gameObject);
        }
        else
        {
            UnHit();
        }
    }

    void UnHit()
    {
        if(hitSomething)
        {
            hitSomething = false;
        }
    }

    void Hit(GameObject g)
    {
        hitSomething = true;
        hitObject = g;
    }
}
