using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public NetworkManager manager;
    public GameObject gameState;
    private string[] towerTypes = { "Tower", "Bunker" };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPractice()
    {
        manager.StartHost();
        SetGameState();

        gameObject.SetActive(false);
        Camera.main.enabled = false;
        gameState.GetComponent<Main>().mode = Modes.SINGLE;
    }

    public void HostGame()
    {
        manager.StartHost();
        DisableTowers();
        SetGameState();
        gameObject.SetActive(false);
        Camera.main.enabled = false;
        gameState.GetComponent<Main>().mode = Modes.MULTI;
    }

    public void JoinGame()
    {
        manager.StartClient();
        DisableTowers();
        SetGameState();
        gameObject.SetActive(false);
        Camera.main.enabled = false;
        gameState.GetComponent<Main>().mode = Modes.MULTI;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void DisableTowers()
    {
        var towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (var tower in towers)
        {
            Destroy(tower);
        }
    }

    void SetGameState()
    {
        gameState = GameObject.FindWithTag("GameState");
    }
}
