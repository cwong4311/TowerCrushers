using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Launch());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reel());
        }
    }

    IEnumerator Launch()
    {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, -90000f);
        yield return new WaitForSeconds(.5f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0f);
    }    

    IEnumerator Reel()
    {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 10f);
        yield return new WaitForSeconds(.5f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0f);
    }   
}
