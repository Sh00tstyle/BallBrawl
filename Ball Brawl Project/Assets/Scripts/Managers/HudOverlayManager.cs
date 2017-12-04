using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudOverlayManager : MonoBehaviour {

	public enum HUDText { CounterTeamRed, CounterTeamBlue, MatchTimer, RoundCountdown, SimpleCrossHair, CooldownText_PlayerPush, CooldownText_Dash, CooldownText_CatchBall }
    public enum HUDImage {
        ScoreboardBackground, BarEmpty, BarFilledRed, BarFilledBlue, Background_PlayerPush, BackgroundInverted_PlayerPush, CooldownFill_PlayerPush, Icon_PlayerPush, IconInverted_PlayerPush,
        KeyQ_PlayerPush, Background_Dash, BackgroundInverted_Dash, CooldownFill_Dash, Icon_Dash, IconInverted_Dash, KeyLShift_Dash, Background_Jetpack, Icon_Jetpack, Icon_Descend, KeySpace_Jetpack,
        KeyLCtrl_Descend, FuelBackground_Jetpack, FuelFill_Jetpack, Background_PushBall, BackgroundInverted_PushBall, CooldownFill_PushBall, Icon_PushBall, IconInverted_PushBall, LeftClick_PushBall,
        Background_CatchBall, BackgroundInverted_CatchBall, CooldownFill_CatchBall, Icon_CatchBall, IconInverted_CatchBall, LeftClick_CatchBall
    }

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
        _hudImages[(int)HUDImage.BarFilledRed].fillAmount = fillAmount;
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
