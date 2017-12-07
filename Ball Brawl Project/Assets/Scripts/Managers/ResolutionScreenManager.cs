using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionScreenManager : MonoBehaviour {

    private static ResolutionScreenManager _instance;

    private enum ResolutionText { TeamRedWon, TeamBlueWon, Draw };

    private Text[] _resolutionTexts;

    public void Awake() {
        _resolutionTexts = GetComponentsInChildren<Text>();
    }

    public void Update() {
        if(GameStateManager.Instance.MatchEnded) {
            DisplayResolutionText();
        } else {
            DisableResolutionTexts();
        }
    }

    public void DisplayResolutionText() {
        int goalsRed = GoalSpawnerScript.Instance.GoalsTeamRed;
        int goalsBlue = GoalSpawnerScript.Instance.GoalsTeamBlue;

        DisableResolutionTexts();

        //it's reversed here
        if(goalsRed < goalsBlue) {
            _resolutionTexts[(int)ResolutionText.TeamRedWon].enabled = true;
        } else if(goalsRed > goalsBlue) {
            _resolutionTexts[(int)ResolutionText.TeamBlueWon].enabled = true;
        } else {
            _resolutionTexts[(int)ResolutionText.Draw].enabled = true;
        }
    }

    private void DisableResolutionTexts() {
        _resolutionTexts[(int)ResolutionText.TeamRedWon].enabled = false;
        _resolutionTexts[(int)ResolutionText.TeamBlueWon].enabled = false;
        _resolutionTexts[(int)ResolutionText.Draw].enabled = false;
    }

    public static ResolutionScreenManager Instance {
        get { return _instance; }
    }
}
