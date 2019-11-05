using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public string phase = "None";
    
    public int p1_towerNum;
    public int p2_towerNum;

    public Canvas p1_buildCanvas;
    public Canvas p1_playCanvas;
    public GameObject p1_selectedTower;
    public int p1_selectedCost;
    public int p1_remainingCost;

    public Camera p1_playCam;
    public Camera p1_buildCam;

    public GameObject p1_catapult;

    private Dictionary<GameObject, int> costForTowers;

    // Start is called before the first frame update
    void Start()
    {
        phase = "build";

        p1_buildCam.gameObject.SetActive(true);
        p1_buildCanvas.gameObject.SetActive(true);
        p1_playCam.gameObject.SetActive(false);
        p1_playCanvas.gameObject.SetActive(false);
        costForTowers = new Dictionary<GameObject, int>();

        p1_catapult.GetComponentInChildren<Catapult>().enabled = false;

        p1_towerNum = 0;
        p2_towerNum = 0;

        p1_selectedCost = 100;
        p1_remainingCost = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == "build") {
            if (Input.GetButtonDown("Fire1")) {
                     //Instantiate(p1_selectedTower, Input.mousePosition, Quaternion.identity);
                    // Construct a ray from the current mouse coordinates
                    Ray ray = p1_buildCam.ScreenPointToRay (Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast (ray, out hit, 100)) {
                        // Create a tower if hit
                        //Debug.Log(hit.collider.gameObject.tag);

                        if (hit.collider.gameObject.tag == "BuildArea")
                            //if p1 has enough gold left
                            if (p1_remainingCost >= p1_selectedCost) {
                                p1_remainingCost -= p1_selectedCost;

                                GameObject newTower = Instantiate (p1_selectedTower, hit.point, transform.rotation);
                                newTower.tag = p1_selectedTower.tag;
                                costForTowers.Add(newTower, p1_selectedCost);
                                p1_towerNum ++;
                            }
                    }
            } else if (Input.GetButtonDown("Fire2")) {
                    Ray ray = p1_buildCam.ScreenPointToRay (Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast (ray, out hit, 100)) {
                        // Destroy a tower if hit and recover its cost
                        if (hit.transform.parent != null && costForTowers.ContainsKey(hit.transform.parent.gameObject)) {
                            Destroy(hit.transform.parent.gameObject);
                            p1_remainingCost += costForTowers[hit.transform.parent.gameObject];
                            costForTowers.Remove(hit.transform.parent.gameObject);
                            p1_towerNum--;
                        }
                    }
            }
        }

        if (phase == "build_done") {
            p1_buildCam.gameObject.SetActive(false);
            p1_buildCanvas.gameObject.SetActive(false);
            p1_playCam.gameObject.SetActive(true);
            p1_playCanvas.gameObject.SetActive(true);

            p2_towerNum = 8;

            p1_catapult.GetComponentInChildren<Catapult>().enabled = true;

            phase = "play";
        }

        if (phase == "play") {
            // Do Nothing for Now
        }
    }
}
