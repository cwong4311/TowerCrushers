using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionSeconds;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, explosionSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
