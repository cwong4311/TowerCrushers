using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    void Update()
    {
        // disable the other players camera so it doesn't interfere with the current players camera
        if (!transform.parent.GetComponent<PlayerController>().isLocalPlayer)
        {
            GetComponent<Camera>().enabled = false;
        }
    }
}
