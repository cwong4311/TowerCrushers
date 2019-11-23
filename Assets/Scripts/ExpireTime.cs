using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpireTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Decay(1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Decay(float wait) 
    {
        yield return new WaitForSeconds(wait);
        Destroy(gameObject);
    }
}
