using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudOverlayManager : MonoBehaviour {

	public enum HUDText { SimpleCrosshair, CaptionTeamA, CaptionTeamB, CounterTeamA, CounterTeamB, MatchTimer, RoundCountdown }
    public enum HUDImage { HoldingBarEmpty, HoldingBarFilled, TeamBackground }

    private static HudOverlayManager _instance;

    private Text[] _hudTexts;
    private Image[] _hudImages;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        _hudTexts = GetComponentsInChildren<Text>();
        _hudImages = GetComponentsInChildren<Image>();
    }

    public void UpdateGoalCount(HUDText hudText, int goals) {
        _hudTexts[(int)hudText].text = "" + goals;
    }

    public void UpdateHoldingBar(float fillAmount) {
        _hudImages[(int)HUDImage.HoldingBarFilled].fillAmount = fillAmount;
    }

    public void UpdateMatchTimer(float time) {
        string minString = "" + Mathf.FloorToInt(time / 60f);

        int sec = Mathf.FloorToInt(time) % 60;
        string secString = "" + sec;

        //Some formatting
        if(sec < 10) {
            secString = "0" + sec;
        } else if(sec == 0) {
            secString = "00";
        }

        _hudTexts[(int)HUDText.MatchTimer].text = minString + ":" + secString;
    }

    public void UpdateMatchTimer(string message) {
        _hudTexts[(int)HUDText.MatchTimer].text = message;
    }

    public void UpdateRoundCountdown(float countdown) {
        _hudTexts[(int)HUDText.RoundCountdown].text = "" + Mathf.CeilToInt(countdown);

        if(countdown <= 0) {
            _hudTexts[(int)HUDText.RoundCountdown].enabled = false;
        } else {
            _hudTexts[(int)HUDText.RoundCountdown].enabled = true;
        }
    }

    public static HudOverlayManager Instance {
        get { return _instance; }
    }
}
