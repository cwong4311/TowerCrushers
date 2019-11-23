using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour 
{
    private readonly float explosionSeconds = 1f;

    void Start()
    {
        CmdDestroyExplosion();
    }

    /*
     * Tell the server to destroy the explosion
     */
    [Command]
    void CmdDestroyExplosion()
    {
        StartCoroutine(DestroyWithDelay());
    }

    /*
     * Wait x seconds then destroy the explosion
     */
    IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(explosionSeconds);
        NetworkServer.Destroy(gameObject);
    }

}
