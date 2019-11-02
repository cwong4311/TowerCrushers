using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Ball" || col.gameObject.name == "Ball(Clone)")
        {
            Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }
}
