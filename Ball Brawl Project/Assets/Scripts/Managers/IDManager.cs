using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IDManager : NetworkBehaviour {

    [SyncVar]
    private int _spawnedPlayers = 0;

    private static IDManager _instance;

    public void Awake() {
        if (_instance == null) {
            _instance = this;
        }
    }

    [Command]
    private void CmdIncreasePlayerCount() {
        _spawnedPlayers++;
    }

    public int GetNextID() {
        CmdIncreasePlayerCount();

        return _spawnedPlayers;
    }

    public static IDManager Instance {
        get { return _instance; }
    }
}
