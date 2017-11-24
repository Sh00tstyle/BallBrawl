using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalScript : NetworkBehaviour {

    [SyncVar]
    private int _goalsScored;

    [SyncVar]
    private bool _isTeamA;

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Tags.BALL) {
            CmdScorePoint();

            if(_isTeamA)  HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamA, _goalsScored);
            else HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamB, _goalsScored);
        }
    }

    [Command]
    public void CmdScorePoint() {
        _goalsScored++;
    }

    public void SetTeam(bool isTeamA) {
        _isTeamA = isTeamA;
    }

    public bool isTeamA {
        get { return _isTeamA; }
    }

    public int goalsScored {
        get { return _goalsScored; }
    }
}
