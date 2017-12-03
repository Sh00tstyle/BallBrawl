using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerScript : NetworkManager {

    private static NetworkManagerScript _instance;

    private int _connectedPlayers;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    private void CloseConnection(NetworkConnection conn) {
        conn.Disconnect();
    }

    //Called when a player is added to a client
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        PlayerSpawnerScript playerSpawner = GetComponent<PlayerSpawnerScript>();

        Vector3 spawnPos;
        Quaternion startRota = Quaternion.identity;

        //Spawns the players based on how many players are connected
        if(_connectedPlayers % 2 == 1) {
            spawnPos = playerSpawner.TeamASpawn.position;
            startRota = Quaternion.Euler(new Vector3(0, 180f, 0));
        } else {
            spawnPos = playerSpawner.TeamBSpawn.position;
        }

        GameObject newPlayer = Instantiate(playerPrefab, spawnPos, startRota);
        NetworkServer.AddPlayerForConnection(conn, newPlayer, playerControllerId);

        PlayerManager.Instance.CmdRegisterPlayer(_connectedPlayers, newPlayer);
    }

    //Called when a client connects
    public override void OnServerConnect(NetworkConnection conn) {
        IncreaseConnectionCount();

        if (_connectedPlayers >= 3) {
            //Prevents more than 2 clients to connect
            conn.Dispose();
            conn.Disconnect();
            return;
        }

        base.OnServerConnect(conn);
    }

    //Called when a player is removed from a client
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
        base.OnServerRemovePlayer(conn, player);
    }

    //Called when a client disconnects
    public override void OnServerDisconnect(NetworkConnection conn) {
        Debug.Log("OnServerDisconnect is called");

        base.OnServerDisconnect(conn);

        //Unregister player from our list
        DecreaseConnectionCount();
        PlayerManager.Instance.CmdUnregisterPlayer(_connectedPlayers);
    }

    private void IncreaseConnectionCount() {
        _connectedPlayers++;
    }

    private void DecreaseConnectionCount() {
        _connectedPlayers--;
    }

    public int SpawnedPlayers {
        get { return _connectedPlayers; }
    }

    public static NetworkManagerScript Instance {
        get { return _instance; }
    }
}
