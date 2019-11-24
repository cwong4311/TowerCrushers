using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameState;
    private bool countStart = true;
    private int timeRemaining = 300;

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

        if (gameState.GetComponent<Main>().p1_buildFinish && gameState.GetComponent<Main>().p2_buildFinish && countStart) 
        {
            countStart = false;
            timeRemaining = 300;
            transform.Find("Timer").Find("Text").GetComponent<Text>().color = Color.black;

            if (gameState.GetComponent<Main>().mode == Modes.SINGLE) {
                timeRemaining = 5999;
            }
            
            StartCoroutine(Countdown());
        }

        int minute = Mathf.RoundToInt(Mathf.Floor(timeRemaining / 60));
        int seconds = timeRemaining - (minute * 60);
        transform.Find("Timer").Find("Text").GetComponent<Text>().text = "Time Left\n" + minute.ToString() + ":" + seconds.ToString("00");
        if (timeRemaining == 20) {
            transform.Find("Timer").Find("Text").GetComponent<Text>().color = Color.red;
        }
        if (timeRemaining == 0) {
            gameState.GetComponent<Main>().SetGameOver(true);
        }
    }

    IEnumerator Countdown()
    {
        while (timeRemaining > 0) {
            yield return new WaitForSeconds(1);
            timeRemaining -= 1;
        }
    }
}
