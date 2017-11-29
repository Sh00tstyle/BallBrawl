using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalScript : NetworkBehaviour {

    [SyncVar]
    private int _goalsScored;

    [SyncVar]
    private string _assignedTeam;

    public override void OnStartClient() {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (_assignedTeam == Teams.TEAM_A) renderer.material.color = Color.red;
        else if (_assignedTeam == Teams.TEAM_B) renderer.material.color = Color.blue;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Tags.BALL) {
            CmdScorePoint();

            if (_assignedTeam == Teams.TEAM_A) {
                HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamA, _goalsScored);
            } else {
                HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamB, _goalsScored);
            }

            GameStateManager.Instance.CmdResetRound(); //Reset the round whenever a goal is scored
        }
    }

    [Command]
    public void CmdScorePoint() {
        _goalsScored++;
    }

    public void SetTeam(string team) {
        _assignedTeam = team;
    }

    public string AssigenedTeam {
        get { return _assignedTeam; }
    }

    public int goalsScored {
        get { return _goalsScored; }
    }
}
