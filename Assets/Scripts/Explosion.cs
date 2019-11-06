using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour 
{
    public float explosionSeconds = 3f;

    // Start is called before the first frame update
    void Start()
    {
        CmdDestroyExplosion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    void CmdDestroyExplosion()
    {
        StartCoroutine(DestroyWithDelay());
    }

    IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(explosionSeconds);
        NetworkServer.Destroy(gameObject);
    }

}
