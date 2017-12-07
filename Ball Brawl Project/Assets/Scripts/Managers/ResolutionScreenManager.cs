using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionScreenManager : MonoBehaviour {

    private static ResolutionScreenManager _instance;

    private enum ResolutionText { TeamRedWon, TeamBlueWon, Draw };

    private Text[] _resolutionTexts;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        _resolutionTexts = GetComponentsInChildren<Text>();
    }

    public void DisplayResolutionText(string winningTeam) {
        DisableResolutionTexts();

        //it's reversed here
        if(winningTeam == Teams.TEAM_RED) {
            _resolutionTexts[(int)ResolutionText.TeamRedWon].enabled = true;
        } else if(winningTeam == Teams.TEAM_BLUE) {
            _resolutionTexts[(int)ResolutionText.TeamBlueWon].enabled = true;
        } else {
            _resolutionTexts[(int)ResolutionText.Draw].enabled = true;
        }
    }

    public void DisableResolutionTexts() {
        _resolutionTexts[(int)ResolutionText.TeamRedWon].enabled = false;
        _resolutionTexts[(int)ResolutionText.TeamBlueWon].enabled = false;
        _resolutionTexts[(int)ResolutionText.Draw].enabled = false;
    }

    public static ResolutionScreenManager Instance {
        get { return _instance; }
    }
}
