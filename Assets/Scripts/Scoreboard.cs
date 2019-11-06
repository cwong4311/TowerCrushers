using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameState;

    void Start()
    {
        gameState = GameObject.FindWithTag("GameState");
    }

    // Update is called once per frame
    void Update()
    {
        var p1_towers = gameState.GetComponent<Main>().p1_towerNum;
        var p2_towers = gameState.GetComponent<Main>().p2_towerNum;

        transform.Find("P1TowerCnt").Find("Text").GetComponent<Text>().text = "P1 Towers: " + p1_towers.ToString();
        transform.Find("P2TowerCnt").Find("Text").GetComponent<Text>().text = "P2 Towers: " + p2_towers.ToString();
    }
}
