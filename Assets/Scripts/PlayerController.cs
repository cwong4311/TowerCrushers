using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject catapultPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdSpawnCatapult();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    void CmdSpawnCatapult()
    {
        GameObject go = Instantiate(catapultPrefab, this.transform.position, Quaternion.identity);


        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }
}
