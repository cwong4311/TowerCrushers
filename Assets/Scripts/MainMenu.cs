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

    public GameObject p1_spawn;
    public GameObject p2_spawn;

    // Start is called before the first frame update
    void Start()
    {
        manager = NetworkManager.singleton;
    }

    /*
     * Start the game in practice mode (single player)
     */
    public void StartPractice()
    {
        manager.StartHost();
        
        EnableTowers();
        SetGameView();

        gameState.GetComponent<Main>().mode = Modes.SINGLE;
        gameState.GetComponent<Main>().Reset();
    }

    /*
     * Display the multiplayer options - host and join game
     */
    public void ShowMultiplayer()
    {
        transform.Find("HostGame").gameObject.SetActive(true);
        transform.Find("JoinGame").gameObject.SetActive(true);

        transform.Find("Practice").gameObject.SetActive(false);
        transform.Find("Multiplayer").gameObject.SetActive(false);
        transform.Find("Exit").gameObject.SetActive(false);
    }

    /*
     * Create a new multiplayer game
     */
    public void HostGame()
    {
        transform.Find("ErrorMsg").gameObject.SetActive(false);

        manager.StartMatchMaker();
        manager.matchMaker.CreateMatch(Random.Range(0, 99999).ToString(), 2, true, "", "", "", 0, 0, OnMatchCreate);
        
        SetGameView();
        gameState.GetComponent<Main>().mode = Modes.MULTI;
        gameState.GetComponent<Main>().Reset();
    }

    /*
     * Disable the single player towers when the match is created
     */
    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        manager.OnMatchCreate(success, extendedInfo, matchInfo);
        DisableTowers();
    }

    /*
     * Join a multiplayer game automatically
     */
    public void JoinGame()
    {
        transform.Find("ErrorMsg").gameObject.SetActive(false);
        manager.StartMatchMaker();
        manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
    }

    /*
     * Find an available match and join it if it exists
     */
    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        // find a match which has vacant slots
        NetworkID nid = NetworkID.Invalid;
        foreach (var match in matches)
        {
            if (match.currentSize < match.maxSize)
            {
                nid = match.networkId;
                break;
            }
        }

        // failed to find a match
        if (nid == NetworkID.Invalid)
        {
            transform.Find("ErrorMsg").gameObject.SetActive(true);
            return;
        } else
        {
            transform.Find("ErrorMsg").gameObject.SetActive(false);
        }

        // succeeded, proceed to join match
        manager.matchMaker.JoinMatch(nid, "", "", "", 0, 0, manager.OnMatchJoined);
        DisableTowers();
        SetGameView();
        gameState.GetComponent<Main>().mode = Modes.MULTI;
        gameState.GetComponent<Main>().Reset();
    }
 
    /*
     * Disconnect a client from the game
     */
    public void Disconnect()
    {

        if (gameState.GetComponent<Main>().mode == Modes.MULTI)
        {
            MatchInfo matchInfo = manager.matchInfo;
            manager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, manager.OnDropConnection);
        }
        manager.StopHost();
    }

    /*
     * Hide the main menu and show the game menu
     */
    void SetGameView()
    {
        transform.Find("RawImage").gameObject.SetActive(false);
        transform.Find("Text").gameObject.SetActive(false);
        transform.Find("HostGame").gameObject.SetActive(false);
        transform.Find("JoinGame").gameObject.SetActive(false);

        transform.Find("Practice").gameObject.SetActive(false);
        transform.Find("Multiplayer").gameObject.SetActive(false);
        transform.Find("Exit").gameObject.SetActive(false);

        transform.Find("MenuCamera").gameObject.SetActive(false);
        transform.Find("Disconnect").gameObject.SetActive(true);
        transform.Find("HelpText").gameObject.SetActive(true);
    }

    /*
     * Hide the game menu and show the main menu
     */
    public void SetMenuView()
    {
        transform.Find("RawImage").gameObject.SetActive(true);
        transform.Find("MenuCamera").gameObject.SetActive(true);
        transform.Find("Text").gameObject.SetActive(true);
        transform.Find("Practice").gameObject.SetActive(true);
        transform.Find("Multiplayer").gameObject.SetActive(true);
        transform.Find("Exit").gameObject.SetActive(true);
        transform.Find("Disconnect").gameObject.SetActive(false);
        transform.Find("HelpText").gameObject.SetActive(false);
        transform.Find("ClientDC").gameObject.SetActive(false);
        resultsCanvas.GetComponent<CheckWin>().SetResultMenuItems(false);
        gameState.GetComponent<Main>().SetGameOver(false);
    }

    /*
     * Exit the game
     */
    public void ExitGame()
    {
        Application.Quit();
    }

    /*
     * Hide the single player towers
     */
    void DisableTowers()
    {
        var towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (var tower in towers)
        {
            tower.SetActive(false);
        }
    }

    /*
     * Show the single player towers
     */
    void EnableTowers()
    {
        var towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (var tower in towers)
        {
            tower.SetActive(true);
        }
    }
}
