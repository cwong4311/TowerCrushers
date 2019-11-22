using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class CustomNetworkManager : NetworkManager {
    private bool player1_taken = false;
    public GameObject p1_spawn;
    public GameObject p2_spawn;
    public GameObject ClientDC_msg;

    // Server callbacks
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        Vector3 location;
        if (!player1_taken) {
            player1_taken = true;
            location = p1_spawn.transform.position;
        } else {
            player1_taken = false;
            location = p2_spawn.transform.position;
        }
        var player = (GameObject)GameObject.Instantiate(playerPrefab, location, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        Debug.Log("Client has requested to get his player added to the game");
    }

    public override void OnStopHost() {
        Debug.Log("Host has stopped");
        player1_taken = false;
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        //NetworkServer.DestroyPlayersForConnection(conn);
        if (conn.lastError != NetworkError.Ok) {
            if (LogFilter.logError) { Debug.LogError("Server Disconnected due to error: " + conn.lastError); }
        }
        Debug.Log("A client disconnected from the server: " + conn);

        ClientDC_msg.SetActive(true);
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError) { 
                Debug.LogError("Client Disconnected due to error: " + conn.lastError);
            }
        }
        Debug.Log("Client disconnected from server: " + conn);

        ClientDC_msg.SetActive(true);
    }
}