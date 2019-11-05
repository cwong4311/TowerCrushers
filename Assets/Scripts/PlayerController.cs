using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject catapultPrefab;
    private LinkedList<Transform> cams = new LinkedList<Transform>();
    private Camera currCam;
    private Camera buildCam;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        currCam = transform.Find("Camera").GetComponent<Camera>();
        //buildCam = transform.Find("dCamera").GetComponent<Camera>();

        Transform t = new GameObject().transform;
        if (!isServer)
        {
            // p2 play cam
            t.position = new Vector3(208.85f, 15.26f, 365.68f);
            t.rotation = Quaternion.Euler(13.871f, 270.729f, 0f);
            cams.AddLast(t);

            // p2 build cam
            t.position = new Vector3(174.2f, 66.5f, 365.4f);
            t.rotation = Quaternion.Euler(90f, 90f, 180f);
            cams.AddLast(t);

            /*
            playCam.transform.position = new Vector3(208.85f, 15.26f, 365.68f);
            playCam.transform.rotation = Quaternion.Euler(13.871f, 270.729f, 0f);

            buildCam.transform.position = new Vector3(174.2f, 66.5f, 365.4f);
            buildCam.transform.rotation = Quaternion.Euler(90f, 90f, 180f);
            */
        } else
        {
            // p1 play cam
            t.position = new Vector3(57.5f, 14.8f, 365.7f);
            t.rotation = Quaternion.Euler(13.871f, 90.00001f, 0f);
            cams.AddLast(t);

            // p1 build cam
            t.position = new Vector3(87.2f, 66.5f, 365.4f);
            t.rotation = Quaternion.Euler(90f, 90.00001f, 0f);
            cams.AddLast(t);

            /*
            playCam.transform.position = new Vector3(57.5f, 14.8f, 365.7f);
            playCam.transform.rotation = Quaternion.Euler(13.871f, 90.00001f, 0f);

            buildCam.transform.position = new Vector3(87.2f, 66.5f, 365.4f);
            buildCam.transform.rotation = Quaternion.Euler(90f, 90.00001f, 0f);
            */
        }
        Transform currTransform = cams.First.Value;
        currCam.transform.position = currTransform.position;
        currCam.transform.rotation = currTransform.rotation;

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
        var cata = Instantiate(catapultPrefab, transform.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(cata, connectionToClient);
    }
}
