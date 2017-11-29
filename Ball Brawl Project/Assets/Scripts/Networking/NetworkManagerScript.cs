using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerScript : NetworkManager {

    private static NetworkManagerScript _instance;

    private int _spawnedPlayers;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        PlayerSpawnerScript playerSpawner = GetComponent<PlayerSpawnerScript>();

        Vector3 spawnPos;

        //Spawns the players based on how many players are connected
        if(_spawnedPlayers % 2 == 0) {
            spawnPos = playerSpawner.TeamASpawn.position;
        } else {
            spawnPos = playerSpawner.TeamBSpawn.position;
        }

        GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, newPlayer, playerControllerId);

        IncreasePlayerCount();
        PlayerManager.Instance.CmdRegisterPlayer(_spawnedPlayers, newPlayer);  
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
        base.OnServerRemovePlayer(conn, player);

        Debug.Log("On Server Remove Player is called");

        //Unregister player from our list
        DecreasePlayerCount();
        PlayerManager.Instance.CmdUnregisterPlayer(_spawnedPlayers);
    }

    private void IncreasePlayerCount() {
        _spawnedPlayers++;
    }

    private void DecreasePlayerCount() {
        _spawnedPlayers--;
    }

    public int SpawnedPlayers {
        get { return _spawnedPlayers; }
    }

    public static NetworkManagerScript Instance {
        get { return _instance; }
    }
}
