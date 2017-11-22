﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalSpawnerScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _goalPrefab;

    [SerializeField]
    private Transform _goalHolder;

    [SerializeField]
    private Transform _goalPosTeamA;

    [SerializeField]
    private Transform _goalPosTeamB;

    public override void OnStartServer() {
        GameObject goalA = Instantiate(_goalPrefab, _goalPosTeamA.position, Quaternion.Euler(new Vector3(90, 0, 0)), _goalHolder);
        GameObject goalB = Instantiate(_goalPrefab, _goalPosTeamB.position, Quaternion.Euler(new Vector3(90, 0, 0)), _goalHolder);

        goalA.name = "GoalTeamA";
        goalB.name = "GoalTeamB";

        goalA.GetComponent<GoalScript>().SetTeam(GoalScript.Team.A);
        goalB.GetComponent<GoalScript>().SetTeam(GoalScript.Team.B);

        NetworkServer.Spawn(goalA);
        NetworkServer.Spawn(goalB);
    }
}
