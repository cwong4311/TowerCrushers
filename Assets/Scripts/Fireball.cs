using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fireball : NetworkBehaviour
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Destructible>() == null)
        {
            CmdExplodeSelf(this.gameObject);
        }
    }

    [Command]
    void CmdExplodeSelf(GameObject go)
    {
        var boom = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(boom);
        NetworkServer.Destroy(go);
    }

}
