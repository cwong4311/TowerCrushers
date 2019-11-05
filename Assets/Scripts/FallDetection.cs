using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetection : MonoBehaviour
{
    private Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(startingPos, transform.position) > 1.5f) {
            StartCoroutine(Fallen());
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "BuildArea" || col.gameObject.tag == "Stage")
        {
            StartCoroutine(Fallen());
        }
    }

    IEnumerator Fallen()
    {
        yield return new WaitForSeconds(5);
        transform.parent.tag = "Debris";
    }
}
