using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Destructible : NetworkBehaviour 
{
    public enum DestructibleBy { Ball,Fireball } // what is required to destroy this object
    public GameObject explosion;
    public GameObject gameState;
    public DestructibleBy destructibleBy;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.FindWithTag("GameState");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (destructibleBy <= DestructibleBy.Ball)
        {
            if (col.gameObject.name == "Ball" || col.gameObject.name == "Ball(Clone)")
            {
                CmdDestroyTower(col.gameObject);
            }
        }
        if (destructibleBy <= DestructibleBy.Fireball)
        {
            if (col.gameObject.name == "Fireball" || col.gameObject.name == "Fireball(Clone)")
            {
                CmdDestroyTower(col.gameObject);
            }
        }
    }

    [Command]
    void CmdDestroyTower(GameObject go)
    {
        var boom = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(boom);
        NetworkServer.Destroy(go); // ball
        
        if (isServer && Time.time - gameState.GetComponent<Main>().p1_next_invincibility < 25)
        {
            return;
        }
        NetworkServer.Destroy(gameObject); // tower 
        DecTowers();
    }

    public void DecTowers()
    {
        if (isServer)
        {
            CmdDecTowers("p1");
        } else
        {
            CmdDecTowers("p2");
        }
    }

    [Command]
    public void CmdDecTowers(string player)
    {
        if (player == "p1")
        {
            gameState.GetComponent<Main>().p1_towerNum--;
        } else if (player == "p2")
        {
            gameState.GetComponent<Main>().p2_towerNum--;
        }
    }
}
