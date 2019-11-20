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

        //transform.Find("JoinGame").GetComponent<Button>().interactable = true;
    }

    public void HostGame()
    {
        //manager.StartHost();
        manager.StartMatchMaker();
        manager.matchMaker.CreateMatch("default", 2, true, "", "", "", 0, 0, OnMatchCreate);

        gameObject.SetActive(false);
        Camera.main.enabled = false;
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

        Debug.Log("disabling...");
        foreach (var tower in towers)
        {
            Debug.Log(tower);
            Destroy(tower);
        }
    }
}
