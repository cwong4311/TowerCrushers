using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Main : NetworkBehaviour
{
    [SyncVar]
    public int p1_towerNum = 0;
    [SyncVar]
    public int p2_towerNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        p1_towerNum = 0;
        p2_towerNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
