using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Destructible : NetworkBehaviour 
{
    public GameObject explosion;
    private readonly float explosionSeconds = 1f;

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
            CmdDestroyTower(col.gameObject);
        }
    }

    [Command]
    void CmdDestroyTower(GameObject go)
    {
        var boom = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(boom);
        NetworkServer.Destroy(go); // ball
        NetworkServer.Destroy(gameObject); // tower 
    }
}
