using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public NetworkManager manager;
    public GameObject gameState;
    private string[] towerTypes = { "Tower", "Bunker", "SmallTower" };

    // Start is called before the first frame update
    void Start()
    {
        var input = transform.Find("InputField").gameObject.GetComponentInChildren<InputField>();
        var se= new InputField.SubmitEvent();
        se.AddListener(SetIPAddress);
        input.onEndEdit = se;
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

    public void ShowMultiplayer()
    {
        transform.Find("InputField").gameObject.SetActive(true);
        transform.Find("HostGame").gameObject.SetActive(true);
        transform.Find("JoinGame").gameObject.SetActive(true);

        transform.Find("Practice").gameObject.SetActive(false);
        transform.Find("Multiplayer").gameObject.SetActive(false);
        transform.Find("Exit").gameObject.SetActive(false);
    }

    public void SetIPAddress(string arg0)
    {
        manager.networkAddress = arg0;

        transform.Find("JoinGame").GetComponent<Button>().interactable = true;
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
