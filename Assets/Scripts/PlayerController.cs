using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject catapultPrefab;
    private GameObject cata;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdSpawnCatapult();
        // position camera behind catapult
        //Camera.main.transform.position = transform.position + (transform.up * 10) - (transform.right * 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    void CmdSpawnCatapult()
    {
        cata = Instantiate(catapultPrefab, transform.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(cata, connectionToClient);
    }
}
