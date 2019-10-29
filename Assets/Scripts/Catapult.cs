using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Catapult : NetworkBehaviour 
{
    public GameObject myBall;
    public Transform ballSpawn;
    private GameObject curBall = null;
    private float ballForce = 0f;
    private float originalForce = 0f;

    // Start is called before the first frame update
    void Start()
    {
        CmdSpawnBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdLaunch();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdReel();
        }
        if (Input.GetKey(KeyCode.D)) {
            if (transform.rotation.y > -0.60f) {
                transform.Rotate(0, -1f, 0);
            }
        }
        if (Input.GetKey(KeyCode.A)) {
            if (transform.rotation.y < 0.60f) {
                transform.Rotate(0, 1f, 0);
            }
        }
        if (Input.GetKey(KeyCode.W)) {
            if (ballForce < (originalForce * 2)) {
                ballForce += (originalForce / 100f);
            }
        }
        if (Input.GetKey(KeyCode.S)) {
            if (ballForce > (originalForce / 20)) {
                ballForce -= (originalForce / 100f);
            }
        }
    }

    [Command]
    public void CmdLaunch()
    {
        StartCoroutine(Launch());
    }

    [Command]
    public void CmdReel()
    {
        StartCoroutine(Reel());
    }

    IEnumerator Launch()
    {
        RpcChangePivotAngularVelocity(new Vector3(0, 0, -90000f));
        yield return new WaitForSeconds(.5f);
        RpcChangePivotAngularVelocity(new Vector3(0, 0, 0f));

        curBall = null;
    }    

    IEnumerator Reel()
    {
        RpcChangePivotAngularVelocity(new Vector3(0, 0, 10f));
        yield return new WaitForSeconds(.5f);
        RpcChangePivotAngularVelocity(new Vector3(0, 0, 0f));
        yield return new WaitForSeconds(1.5f);

        CmdSpawnBall();
    }

    [ClientRpc]
    public void RpcChangePivotAngularVelocity(Vector3 v)
    {
        var pipe = transform.Find("Pivot").Find("Pipe").gameObject;
        pipe.GetComponent<Rigidbody>().angularVelocity = v;
    }

    [Command]
    private void CmdSpawnBall() {
        var ball = Instantiate(myBall, ballSpawn.position, Quaternion.identity);
        curBall = ball;
        originalForce = curBall.GetComponent<ConstantForce>().force.y;
        ballForce = originalForce;
        curBall.GetComponent<ConstantForce>().force = new Vector3(0, ballForce, 0);

        NetworkServer.Spawn(curBall);
    }
}
