using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManagerScript : MonoBehaviour {

    public void UpdateUIGoals(GoalScript.Team team, int score) {
        if(team == GoalScript.Team.A) {
            HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamA, score);
        } else {
            HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamB, score);
        }
    }
}
