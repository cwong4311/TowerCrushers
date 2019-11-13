using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum Phases { BUILD, PLAY };
public enum Modes { SINGLE, MULTI };

public class PlayerController : NetworkBehaviour
{
    public GameObject gameState;
    public GameObject catapultPrefab;
    public GameObject myCatapult;
    public GameObject selectedTower;
    public GameObject fireballObj;
    public int selectedCost;

    public int currency = 1000;
    public Phases phase = Phases.BUILD;
    public Camera currCam;
    public Canvas buildCanvas;
    public Canvas playCanvas;
    public Slider slider;

    private readonly float multiplierStep = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.FindWithTag("GameState");
        if (!isLocalPlayer)
        {
            return;
        }

        SetBuildCamera();

        CmdSpawnCatapult();
        myCatapult.GetComponent<Catapult>().multiplier = slider.value;
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

                            SpawnTower(hit.point);
                        }
                    }
                }
            }

        }

        if (phase == Phases.PLAY)
        {
            SetPlayCamera();

            if (Input.GetKey(KeyCode.W))
            {
                if (myCatapult.GetComponent<Catapult>().multiplier < 3)
                {
                    myCatapult.GetComponent<Catapult>().multiplier += multiplierStep;
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (myCatapult.GetComponent<Catapult>().multiplier > 1)
                {
                    myCatapult.GetComponent<Catapult>().multiplier -= multiplierStep;
                }
            }
        }

        if (Input.GetKey(KeyCode.P))
        {
            phase = Phases.PLAY;
        }

        if (Input.GetKey(KeyCode.B))
        {
            phase = Phases.BUILD;
        }

        slider.value = myCatapult.GetComponent<Catapult>().multiplier;
    }

    public void ShootFireball(Vector3 origin, Vector3 direction)
    {
        CmdShootFireball(currCam.transform.position, direction);
    }

    [Command]
    void CmdShootFireball(Vector3 origin, Vector3 direction)
    {
        // FromToRotation(new Vector3(0, 0, 1), direction)
        var netFireball = Instantiate(fireballObj, origin, Quaternion.identity);
        netFireball.GetComponent<ConstantForce>().force = direction;
        NetworkServer.Spawn(netFireball);
    }

    [Command]
    void CmdSpawnCatapult()
    {
        myCatapult = Instantiate(catapultPrefab, transform.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(myCatapult, connectionToClient);
    }

    void SpawnTower(Vector3 pos)
    {
        CmdSpawnTower(pos);
        IncTowers();
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

    public void IncTowers()
    {
        if (isServer)
        {
            CmdIncTowers("p1");
        } else
        {
            CmdIncTowers("p2");
        }
    }

    [Command]
    public void CmdIncTowers(string player)
    {
        if (player == "p1")
        {
            gameState.GetComponent<Main>().p1_towerNum++;
        } else if (player == "p2")
        {
            gameState.GetComponent<Main>().p2_towerNum++;
        }
    }
}
