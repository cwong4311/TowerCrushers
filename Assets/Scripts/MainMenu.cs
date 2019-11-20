using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    private NetworkManager manager;
    public GameObject gameState;
    public GameObject resultsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        manager = NetworkManager.singleton;
        var input = transform.Find("InputField").gameObject.GetComponentInChildren<InputField>();
        var se = new InputField.SubmitEvent();
        se.AddListener(SetIPAddress);
        input.onEndEdit = se;
    }

    public void StartPractice()
    {
        manager.StartHost();

        SetGameView();
        gameState.GetComponent<Main>().Reset();
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

        //transform.Find("JoinGame").GetComponent<Button>().interactable = true;
    }

    public void HostGame()
    {
        manager.StartMatchMaker();
        manager.matchMaker.CreateMatch("default", 2, true, "", "", "", 0, 0, OnMatchCreate);

        SetGameView();
        gameState.GetComponent<Main>().Reset();
        gameState.GetComponent<Main>().mode = Modes.MULTI;
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        manager.OnMatchCreate(success, extendedInfo, matchInfo);
        DisableTowers();
    }

    public void JoinGame()
    {
        manager.StartMatchMaker();
        manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        NetworkID nid = NetworkID.Invalid;
        foreach (var match in matches)
        {
            if (match.currentSize < match.maxSize)
            {
                nid = match.networkId;
                break;
            }
        }

        if (nid == NetworkID.Invalid)
        {
            Debug.Log("failed to find game");
            return;
        }

        manager.matchMaker.JoinMatch(nid, "", "", "", 0, 0, manager.OnMatchJoined);
        DisableTowers();
        SetGameView();
        gameState.GetComponent<Main>().Reset();
        gameState.GetComponent<Main>().mode = Modes.MULTI;
    }

    public void Disconnect()
    {

        if (gameState.GetComponent<Main>().mode == Modes.MULTI)
        {
            MatchInfo matchInfo = manager.matchInfo;
            manager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, manager.OnDropConnection);
        }
        manager.StopHost();
    }

    void SetGameView()
    {
        transform.Find("Text").gameObject.SetActive(false);
        transform.Find("InputField").gameObject.SetActive(false);
        transform.Find("HostGame").gameObject.SetActive(false);
        transform.Find("JoinGame").gameObject.SetActive(false);

        transform.Find("Practice").gameObject.SetActive(false);
        transform.Find("Multiplayer").gameObject.SetActive(false);
        transform.Find("Exit").gameObject.SetActive(false);

        transform.Find("MenuCamera").gameObject.SetActive(false);
        transform.Find("Disconnect").gameObject.SetActive(true);
    }

    public void SetMenuView()
    {
        transform.Find("MenuCamera").gameObject.SetActive(true);
        transform.Find("Text").gameObject.SetActive(true);
        transform.Find("Practice").gameObject.SetActive(true);
        transform.Find("Multiplayer").gameObject.SetActive(true);
        transform.Find("Exit").gameObject.SetActive(true);
        transform.Find("Disconnect").gameObject.SetActive(false);
        resultsCanvas.GetComponent<CheckWin>().SetResultMenuItems(false);
        gameState.GetComponent<Main>().SetGameOver(false);
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
}
