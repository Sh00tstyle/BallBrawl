using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalSpawnerScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _goalPrefab;

    [SerializeField]
    private Transform _goalPosTeamA;

    [SerializeField]
    private Transform _goalPosTeamB;

    private static GoalSpawnerScript _instance;

    private GoalScript _goalTeamRed;
    private GoalScript _goalTeamBlue;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    public override void OnStartServer() {
        GameObject goalA = Instantiate(_goalPrefab, _goalPosTeamA.position, Quaternion.Euler(new Vector3(90, 0, 0)));
        GameObject goalB = Instantiate(_goalPrefab, _goalPosTeamB.position, Quaternion.Euler(new Vector3(90, 0, 0)));

        _goalTeamRed = goalA.GetComponent<GoalScript>();
        _goalTeamRed.SetTeam(Teams.TEAM_RED);

        _goalTeamBlue = goalB.GetComponent<GoalScript>();
        _goalTeamBlue.SetTeam(Teams.TEAM_BLUE);

        NetworkServer.Spawn(goalA);
        NetworkServer.Spawn(goalB);
    }

    public void ResetGoals() {
        _goalTeamRed.CmdReset();
        _goalTeamBlue.CmdReset();

        HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamA, 0);
        HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamB, 0);
    }

    public static GoalSpawnerScript Instance {
        get { return _instance; }
    }
}
