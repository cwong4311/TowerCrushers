using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    public GameObject myBall;
    private GameObject curBall = null;
    private float ballForce = 0f;
    private float originalForce = 0f;

    // Start is called before the first frame update
    void Start()
    {
        setBall(Instantiate(myBall, transform.Find("Spawn").position, Quaternion.identity));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Launch());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reel());
        }
        if (Input.GetKey(KeyCode.D)) {
            if (transform.parent.rotation.y > -0.60f) {
                transform.parent.Rotate(0, -1f, 0);
            }
        }
        if (Input.GetKey(KeyCode.A)) {
            if (transform.parent.rotation.y < 0.60f) {
                transform.parent.Rotate(0, 1f, 0);
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

    IEnumerator Launch()
    {
        curBall.GetComponent<ConstantForce>().force = new Vector3(0, ballForce, 0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, -90000f);
        yield return new WaitForSeconds(.5f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0f);
        curBall = null;
    }    

    IEnumerator Reel()
    {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 10f);
        yield return new WaitForSeconds(.5f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0f);
        yield return new WaitForSeconds(1.5f);
        setBall(Instantiate(myBall, transform.Find("Spawn").position, Quaternion.identity));
    }

    private void setBall(GameObject newBall) {
        curBall = newBall;
        originalForce = curBall.GetComponent<ConstantForce>().force.y;
        ballForce = originalForce;
    }
}
