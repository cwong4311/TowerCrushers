using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLife : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "BuildArea" || col.gameObject.tag == "Stage")
        {
            StartCoroutine(Expire());
        }
    }

    IEnumerator Expire()
    {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }
}
