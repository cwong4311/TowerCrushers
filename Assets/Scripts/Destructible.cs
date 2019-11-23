using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Destructible : NetworkBehaviour 
{
    public enum DestructibleBy { Ball,Fireball } // what is required to destroy this object
    public GameObject explosion;
    public GameObject gameState;
    public GameObject barrier;
    public DestructibleBy destructibleBy;
    public Material damagedMaterial;
    public Material healthyMaterial;
    [SyncVar]
    public int health = 1;
    public bool isTower;
    private bool hitDelay = false;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.FindWithTag("GameState");
        Debug.Log(gameObject.name);
        if (gameObject.name.StartsWith("SmallTower") || gameObject.name.StartsWith("Bunker"))
        {
            health = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer) {
            if (health > 1)
            {
                RpcChangeMat("healthy");
            } else
            {
                RpcChangeMat("damaged");
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
        if (hitDelay) return;
        hitDelay = true;
        StartCoroutine(DelayConsecutiveHit(0.2f));

        var boom = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(boom);
        NetworkServer.Destroy(go); // ball

        var hitColliders = Physics.OverlapSphere(gameObject.transform.position, 1);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].name == "P1_Area")
            {
                if (gameState.GetComponent<Main>().p1_invincible)
                {
                    RpcSpawnBarrier(gameObject);
                    return;
                }

                health--;

                if (health <= 0) {
                    if (isTower) CmdDecTowers("p1");
                    NetworkServer.Destroy(gameObject); // tower
                }
            }
            else if (hitColliders[i].name == "P2_Area")
            {
                if (gameState.GetComponent<Main>().p2_invincible)
                {
                    RpcSpawnBarrier(gameObject);
                    return;
                }

                health--;

                if (health <= 0) {
                    if (isTower) CmdDecTowers("p2");
                    NetworkServer.Destroy(gameObject); // tower
                }
            }
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
    [ClientRpc]
    void RpcSpawnBarrier(GameObject location) {
        var shield = Instantiate(barrier, location.transform.position, Quaternion.identity);
        shield.GetComponent<ExpireTime>().StartLife(1f);
    }

    [ClientRpc]
    void RpcChangeMat(string health)
    {
        MeshRenderer[] walls = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer w in walls)
        {
            if (health == "healthy")
            {
                w.material = healthyMaterial;        // this is the line that actually makes the change in color happen
            } else if (health == "damaged")
            {
                w.material = damagedMaterial; 
            }
        }
    }

    IEnumerator DelayConsecutiveHit(float wait) {
        yield return new WaitForSeconds(wait);
        hitDelay = false;
    }
}
