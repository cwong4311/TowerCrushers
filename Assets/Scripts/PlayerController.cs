using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum Phases { BUILD, WAIT, PLAY, FORTIFY };
public enum Modes { SINGLE, MULTI };

public class PlayerController : NetworkBehaviour
{
    public GameObject gameState;
    public GameObject menuCanvas;
    public GameObject catapultPrefab;
    public GameObject myCatapult;
    public string selectedTower;

    public GameObject towerPrefab;
    public GameObject bunkerPrefab;
    public GameObject wallPrefab;
    public GameObject smallTowerPrefab;

    public GameObject fireballObj;
    public int selectedCost;

    public int currency = 1000;
    public Phases phase = Phases.BUILD;
    public Camera currCam;
    public Canvas buildCanvas;
    public Canvas playCanvas;
    public Canvas fortifyCanvas;
    public Canvas errorCanvas;
    public Slider slider;

    private float rechargeRate = 2;
    private float moneyInc = 0;
    public Collider divider;

    private bool buildable = false;
    private bool controlHelpActive = false;
    private bool tutorialPlayed = false;

    private readonly float multiplierStep = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.FindWithTag("GameState");
        menuCanvas = GameObject.FindWithTag("MainMenu");
        divider = GameObject.FindWithTag("Divider").GetComponent<Collider>();

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
        if (!hasAuthority || !isLocalPlayer)
        {
            return;
        }

        // wait for another player to join the game
        if (isServer && (NetworkServer.connections.Count != 2) && gameState.GetComponent<Main>().mode != Modes.SINGLE)
        {
            errorCanvas.gameObject.SetActive(true);
            return;
        } else
        {
            errorCanvas.gameObject.SetActive(false);
        }

        int p1_towers = gameState.GetComponent<Main>().p1_towerNum;
        int p2_towers = gameState.GetComponent<Main>().p2_towerNum;

        // end the game if either player has no more towers remaining
        if ((p1_towers == 0 || p2_towers == 0) && !(phase == Phases.BUILD || phase == Phases.WAIT))
        {
            gameState.GetComponent<Main>().SetGameOver(true);
        }

        // show help
        if (Input.GetKeyDown(KeyCode.F1))
        {
            controlHelpActive = !controlHelpActive;
            menuCanvas.transform.Find("ControlsPopUp").gameObject.SetActive(controlHelpActive);
        }

        // passively increase currency
        if (phase == Phases.PLAY || phase == Phases.FORTIFY) 
        {
            if (Time.time > moneyInc) {
                currency += 5;
                moneyInc = Time.time + rechargeRate;
            }
        } else
        {
            moneyInc = Time.time + rechargeRate;
        }

        // initial build tower/bunker/small tower phase
        if (phase == Phases.BUILD)
        {
            SetBuildCamera();
            CheckTowerSpawn(true);
            divider.enabled = true;
        }

        // wait for the other player to finish placing their towers
        if (phase == Phases.WAIT)
        {
            SetWaiting();
            if (gameState.GetComponent<Main>().p1_buildFinish == true && gameState.GetComponent<Main>().p2_buildFinish == true)
            {
                phase = Phases.PLAY;
                divider.enabled = false;
            }
        }

        // build mode (only walls) after the initial build phase
        if (phase == Phases.FORTIFY)
        {
            SetFortifyCamera();
            CheckTowerSpawn(false);
        }

        // control the catapult
        if (phase == Phases.PLAY)
        {
            SetPlayCamera();
            
            // increase the force of the catapult
            if (Input.GetKey(KeyCode.W))
            {
                if (myCatapult.GetComponent<Catapult>().multiplier < 4)
                {
                    IncForce(multiplierStep);                                           //Server increment
                    myCatapult.GetComponent<Catapult>().multiplier += multiplierStep;   //Local Increment
                }
            }

            // decrease the force of the catapult
            if (Input.GetKey(KeyCode.S))
            {
                if (myCatapult.GetComponent<Catapult>().multiplier > 1)
                {
                    IncForce(-multiplierStep);
                    myCatapult.GetComponent<Catapult>().multiplier -= multiplierStep;
                }
            }
        }

        // switch to play mode
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (phase == Phases.FORTIFY) {
                phase = Phases.PLAY;
            }
        }

        // switch to build mode
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (phase == Phases.PLAY) {
                selectedCost = 0;
                selectedTower = null;
                phase = Phases.FORTIFY;
            }
        }

        // toggle between modes
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (phase == Phases.PLAY) {
                selectedCost = 0;
                selectedTower = null;
                phase = Phases.FORTIFY;
            } else if (phase == Phases.FORTIFY) {
                phase = Phases.PLAY;
            }
        }

        slider.value = myCatapult.GetComponent<Catapult>().multiplier;
    }

    /*
     * Check if a unit can spawn on the area the player clicked on
     */
    void CheckTowerSpawn(bool isTower)
    {
        if (Input.GetButtonDown("Fire1") && buildable)
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

                        SpawnTower(hit.point, isTower);

                        if (isServer)
                        {
                            StartCoroutine(BuildSync(selectedTower, selectedCost, 0.2f));
                        } else
                        {
                            StartCoroutine(BuildSync(selectedTower, selectedCost, 1f));                           
                        }
                    }
                }
            }
        }

    }

    /*
     * Activate invincibility for a player
     */
    public void ActivateInvincibility()
    {
        if (isServer)
        {
            CmdActivateInvincibility("p1");
        } else 
        {
            CmdActivateInvincibility("p2");
        }
    }

    /*
     * Tell the server to activate invincibility
     */
    [Command]
    public void CmdActivateInvincibility(string player)
    {
        if (player == "p1")
        {
            gameState.GetComponent<Main>().p1_invincible = true;
        } else if (player == "p2")
        {
            gameState.GetComponent<Main>().p2_invincible = true;
        }
    }

    /* 
     * Deactivate invincibility for a player
     */
    public void DeactivateInvincibility()
    {
        if (isServer)
        {
            CmdDeactivateInvincibility("p1");
        } else 
        {
            CmdDeactivateInvincibility("p2");
        }
    }

    /* 
     * Tell the server to deactivate invincibility
     */
    [Command]
    public void CmdDeactivateInvincibility(string player)
    {
        if (player == "p1")
        {
            gameState.GetComponent<Main>().p1_invincible = false;
        } else if (player == "p2")
        {
            gameState.GetComponent<Main>().p2_invincible = false;
        }
    }

    /*
     * Spawn a fireball
     */
    public void ShootFireball(Vector3 origin, Vector3 direction)
    {
        CmdShootFireball(currCam.transform.position, direction);
    }

    /*
     * Tell the server to spawn the fireball
     */
    [Command]
    void CmdShootFireball(Vector3 origin, Vector3 direction)
    {
        Quaternion shootRotation = Quaternion.LookRotation(direction, Vector3.up);
        // FromToRotation(new Vector3(0, 0, 1), direction)
        var netFireball = Instantiate(fireballObj, origin, shootRotation);
        netFireball.GetComponent<ConstantForce>().force = direction * 3;
        NetworkServer.SpawnWithClientAuthority(netFireball, connectionToClient);
    }

    /*
     * Tell the server to spawn the player's catapult
     */
    [Command]
    void CmdSpawnCatapult()
    {
        myCatapult = Instantiate(catapultPrefab, transform.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(myCatapult, connectionToClient);
    }

    /* 
     * Set the unit to spawn
     */
    public void SetMyTower(string tower_name, int tower_cost) {
        selectedTower = "";
        selectedCost = 0;
        StartCoroutine(BuildSync(tower_name, tower_cost, 0.2f));
    }

    /*
     * Wait x amount of time for a player to build another unit
     */
    IEnumerator BuildSync(string tower_name, int tower_cost, float wait) {
        buildable = false;
        selectedTower = tower_name;
        selectedCost = tower_cost;
        yield return new WaitForSeconds(wait);
        buildable = true;
    }

    /*
     * Spawn the unit
     */
    void SpawnTower(Vector3 pos, bool isTower)
    {
        CmdSpawnTower(selectedTower, pos);
        if (isTower) IncTowers();
    }

    /*
     * Tell the server to spawn a unit
     */
    [Command]
    void CmdSpawnTower(string chosenTower, Vector3 pos)
    {        
        GameObject myTower = null;
        switch(chosenTower) {
            case "Wall":
                myTower = wallPrefab;
                break;
            case "Bunker":
                myTower = bunkerPrefab;
                break;
            case "SmallTower":
                myTower = smallTowerPrefab;
                break;
            case "Tower":
                myTower = towerPrefab;
                break;
            default:
                break;
        }
        if (myTower != null) {
            var tower = Instantiate(myTower, pos, transform.rotation);
            NetworkServer.SpawnWithClientAuthority(tower, connectionToClient);
        }
    }

    /*
     * Setup the build mode cameras and menus
     */
    private void SetBuildCamera()
    {
        buildCanvas.gameObject.SetActive(true);
        fortifyCanvas.gameObject.SetActive(false);
        playCanvas.gameObject.SetActive(false);
        if (isServer)
        {
            // p1 build cam
            currCam.transform.position = new Vector3(87.2f, 66.5f, 365.6f);
            currCam.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
        } else
        {
            // p2 build cam
            currCam.transform.position = new Vector3(178.2f, 66.5f, 365.6f);
            currCam.transform.rotation = Quaternion.Euler(90f, 90f, 180f);
        }

        // Show tutorial if practice mode
        if (gameState.GetComponent<Main>().mode == Modes.SINGLE && !tutorialPlayed) {
            buildCanvas.transform.Find("Tutorial1").gameObject.SetActive(true);
            tutorialPlayed = true;
        }
    }

    /*
     * Load the waiting text
     */
    private void SetWaiting()
    {
        buildCanvas.transform.Find("LoadText").gameObject.SetActive(true);
    }

    /*
     * Setup the fortify mode cameras and menus
     */
    private void SetFortifyCamera()
    {
        buildCanvas.gameObject.SetActive(false);
        fortifyCanvas.gameObject.SetActive(true);
        playCanvas.gameObject.SetActive(false);
        if (isServer)
        {
            // p1 build cam
            currCam.transform.position = new Vector3(87.2f, 66.5f, 365.6f);
            currCam.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
        } else
        {
            // p2 build cam
            currCam.transform.position = new Vector3(178.2f, 66.5f, 365.6f);
            currCam.transform.rotation = Quaternion.Euler(90f, 90f, 180f);
        }
    }

    /*
     * Setup the play mode cameras and menus
     */
    private void SetPlayCamera()
    {
        buildCanvas.gameObject.SetActive(false);
        fortifyCanvas.gameObject.SetActive(false);
        playCanvas.gameObject.SetActive(true);
        if (isServer)
        {
            // p1 play cam
            currCam.transform.position = new Vector3(57.5f, 15f, 365f);
            currCam.transform.rotation = Quaternion.Euler(14f, 90f, 0f);
        } else
        {
            // p2 play cam
            currCam.transform.position = new Vector3(208.85f, 15f, 365f);
            currCam.transform.rotation = Quaternion.Euler(14f, 270f, 0f);
        }        
        
        // Show tutorial if practice mode
        if (gameState.GetComponent<Main>().mode == Modes.SINGLE && tutorialPlayed) {
            playCanvas.transform.Find("Tutorial2").gameObject.SetActive(true);
            tutorialPlayed = false;
        }
    }

    /*
     * Increase the force of the catapult
     */
    void IncForce(float increment) {
        CmdIncForce(increment);
    }

    /*
     * Tell the server to increase the force of the catapult
     */
    [Command]
    void CmdIncForce(float increment)
    {
        myCatapult.GetComponent<Catapult>().multiplier += increment;
    }

    /*
     * Increment the amount of towers a player has spawned
     */
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

    /*
     * Tell the server to increment the amount of towers a player has spawned
     */
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

    /*
     * Tell the server that a player has finished placing towers
     */
    public void FinishBuilding(bool state)
    {
        if (isServer)
        {
            CmdFinishBuilding("p1", state);
        } else
        {
            CmdFinishBuilding("p2", state);
        }        
    }
    
    /*
     * Tell the server that a player has finished placing towers
     */
    [Command]
    public void CmdFinishBuilding(string player, bool state)
    {
        if (player == "p1")
        {
            gameState.GetComponent<Main>().p1_buildFinish = state;
        } else if (player == "p2")
        {
            gameState.GetComponent<Main>().p2_buildFinish = state;
        }
    }
}
