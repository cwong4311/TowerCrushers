using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum Phases { BUILD, PLAY };

public class PlayerController : NetworkBehaviour
{
    public GameObject catapultPrefab;
    public GameObject selectedTower;
    public int selectedCost;

    public int currency = 1000;
    public Phases phase;
    public Camera currCam;
    public Canvas buildCanvas;
    public Canvas playCanvas;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        phase = Phases.BUILD;

        SetBuildCamera();

        CmdSpawnCatapult();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (phase == Phases.BUILD)
        {
            SetBuildCamera();
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = currCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    // Create a tower if hit
                    //Debug.Log(hit.collider.gameObject.tag);

                    if (hit.collider.gameObject.tag == "BuildArea")
                    {
                        if (currency >= selectedCost)
                        {
                            currency -= selectedCost;

                            CmdSpawnTower(hit.point);
                            //p1_towerNum ++;
                        }
                    }
                }
            }

        }

        if (phase == Phases.PLAY)
        {
            SetPlayCamera();
        }

        if (Input.GetKey(KeyCode.P))
        {
            phase = Phases.PLAY;
        }

        if (Input.GetKey(KeyCode.B))
        {
            phase = Phases.BUILD;
        }
    }

    [Command]
    void CmdSpawnCatapult()
    {
        var cata = Instantiate(catapultPrefab, transform.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(cata, connectionToClient);
    }

    [Command]
    void CmdSpawnTower(Vector3 pos)
    {
        var tower = Instantiate(selectedTower, pos, transform.rotation);
        NetworkServer.Spawn(tower);
    }

    private void SetBuildCamera()
    {
        buildCanvas.gameObject.SetActive(true);
        playCanvas.gameObject.SetActive(false);
        if (isServer)
        {
            // p1 build cam
            currCam.transform.position = new Vector3(87.2f, 66.5f, 365.4f);
            currCam.transform.rotation = Quaternion.Euler(90f, 90.00001f, 0f);
        } else
        {
            // p2 build cam
            currCam.transform.position = new Vector3(174.2f, 66.5f, 365.4f);
            currCam.transform.rotation = Quaternion.Euler(90f, 90f, 180f);
        }
    }

    private void SetPlayCamera()
    {
        buildCanvas.gameObject.SetActive(false);
        playCanvas.gameObject.SetActive(true);
        if (isServer)
        {
            // p1 play cam
            currCam.transform.position = new Vector3(57.5f, 14.8f, 365.7f);
            currCam.transform.rotation = Quaternion.Euler(13.871f, 90.00001f, 0f);
        } else
        {
            // p2 play cam
            currCam.transform.position = new Vector3(208.85f, 15.26f, 365.68f);
            currCam.transform.rotation = Quaternion.Euler(13.871f, 270.729f, 0f);
        }
    }
}
