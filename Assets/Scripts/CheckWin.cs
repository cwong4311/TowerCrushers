using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckWin : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameState;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.GetComponent<Main>().gameOver)
        {
            int p1_towers = gameState.GetComponent<Main>().p1_towerNum;
            int p2_towers = gameState.GetComponent<Main>().p2_towerNum;

            var winner = (p1_towers > p2_towers) ? "PLAYER 1" : "PLAYER 2";
            transform.Find("Text").GetComponent<Text>().text = winner + " WINS!";

            mainMenu.GetComponent<MainMenu>().Disconnect();
            mainMenu.transform.Find("Disconnect").gameObject.SetActive(false);
            SetResultMenuItems(true);
        }
    }

    public void SetResultMenuItems(bool status)
    {
        transform.Find("Text").gameObject.SetActive(status);
        transform.Find("EndMenu").gameObject.SetActive(status);
        transform.Find("EndExit").gameObject.SetActive(status);
    }
}
