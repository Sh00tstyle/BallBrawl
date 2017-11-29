﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamManager : NetworkBehaviour {

    public static string TEAM_A = "TEAM_A";
    public static string TEAM_B = "TEAM_B";

    [SyncVar]
    private int _spawnedPlayers = 0;

    private static TeamManager _instance;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    [Command]
    private void CmdIncreasePlayerCount() {
        _spawnedPlayers++;
    }

    public string GetTeam() {
        CmdIncreasePlayerCount();

        if ((_spawnedPlayers - 1) % 2 == 0) return TEAM_A;
        else return TEAM_B;
    }

    public static TeamManager Instance {
        get { return _instance; }
    }
}