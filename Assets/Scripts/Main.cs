using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Main : NetworkBehaviour
{
    public Modes mode;
    [SyncVar]
    public int p1_towerNum = 0;
    [SyncVar]
    public int p2_towerNum = 0;

    public float invincibility_cooldown = 30;
    [SyncVar]
    public float p1_next_invincibility = 0;
    [SyncVar]
    public float p2_next_invincibility = 0;

    // Start is called before the first frame update
    void Start()
    {
        p1_towerNum = 0;
        p2_towerNum = 0;
        if (mode == Modes.SINGLE)
        {
            p2_towerNum = 8;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
