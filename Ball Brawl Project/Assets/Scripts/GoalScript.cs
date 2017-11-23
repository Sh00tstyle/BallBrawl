using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalScript : NetworkBehaviour {

	public enum Team { A, B };

    [SyncVar]
    private int _goalsScored;

    private Team _goalTeam;
    private GoalManagerScript _goalManager;

    public void Awake() {
        _goalManager = GetComponentInParent<GoalManagerScript>();
    }

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == Tags.BALL) CmdScorePoint();
    }

    [Command]
    public void CmdScorePoint() {
        _goalsScored++;
        _goalManager.UpdateUIGoals(_goalTeam, _goalsScored);

        Debug.Log("Team " + _goalTeam + " now has " + _goalsScored + " goals");
    }

    public void SetTeam(Team team) {
        _goalTeam = team;
    }

    public Team goalTeam {
        get { return _goalTeam; }
    }

    public int goalsScored {
        get { return _goalsScored; }
    }
}
