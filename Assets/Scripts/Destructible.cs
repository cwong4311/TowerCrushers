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
    public Material damagedMaterial;
    public Material healthyMaterial;
    [SyncVar]
    public int health = 2;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.FindWithTag("GameState");
        Debug.Log(gameObject.name);
        if (gameObject.name.StartsWith("SmallTower") || gameObject.name.StartsWith("Bunker"))
        {
            health = 4;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 2)
        {
            MeshRenderer[] walls = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer w in walls)
            {
                w.material = healthyMaterial;
            }
        } else
        {
            Component[] walls = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer w in walls)
            {
               w.material = damagedMaterial;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (destructibleBy <= DestructibleBy.Ball)
        {
            if (col.gameObject.name == "Ball" || col.gameObject.name == "Ball(Clone)")
            {
                CmdDestroyTower(col.gameObject);
                return;
            }
        }
        
        if (destructibleBy <= DestructibleBy.Fireball)
        {
            if (col.gameObject.name == "Fireball" || col.gameObject.name == "Fireball(Clone)")
            {
                CmdDestroyTower(col.gameObject);
                return;
            }
        }
    }

    [Command]
    void CmdDestroyTower(GameObject go)
    {
        Debug.Log("asd");
        var boom = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(boom);
        NetworkServer.Destroy(go); // ball
        
        if (isServer && gameState.GetComponent<Main>().p1_invincible)
        {
            return;
        }
        if (isServer && gameState.GetComponent<Main>().p2_invincible)
        {
            return;
        }

        health--;
        if (health <= 0)
        {
            var hitColliders = Physics.OverlapSphere(gameObject.transform.position, 1);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].name == "P1_Area")
                {
                    CmdDecTowers("p1");
                }
                else if (hitColliders[i].name == "P2_Area")
                {
                    CmdDecTowers("p2");
                }
            }
            NetworkServer.Destroy(gameObject); // tower 
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
