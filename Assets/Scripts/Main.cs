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
    public bool gameOver = false;

    public float invincibility_cooldown = 30;
    [SyncVar]
    public bool p1_invincible = false;
    [SyncVar]
    public bool p2_invincible = false;

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

    public void Reset()
    {
        p1_towerNum = 0;
        p2_towerNum = 0;
        gameOver = false;
        p1_invincible = false;
        p2_invincible = false;

        if (mode == Modes.SINGLE)
        {
            p2_towerNum = 8;
        }
    }

    public void SetGameOver(bool status)
    {
        gameOver = status;
    }

    [Command]
    public void CmdSetGameOver(bool status)
    {
        SetGameOver(status);
    }
}
