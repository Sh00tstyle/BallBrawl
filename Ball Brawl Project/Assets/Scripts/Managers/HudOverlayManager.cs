using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudOverlayManager : MonoBehaviour {

    public enum HUDText { CounterTeamRed, CounterTeamBlue, MatchTimer, RoundCountdown, CooldownText_PlayerPush, CooldownText_Dash, CooldownText_CatchBall }
    public enum HUDImage {
        ScoreboardBackground, CrosshairBase, CrosshairOutline, CrosshairMaxRange, Background_PlayerPush, BackgroundInverted_PlayerPush, Icon_PlayerPush, IconInverted_PlayerPush, CooldownFill_PlayerPush,
        KeyQ_PlayerPush, Background_Dash, BackgroundInverted_Dash, Icon_Dash, IconInverted_Dash, CooldownFill_Dash, KeyLShift_Dash, Background_Jetpack, Icon_Jetpack, Icon_Descend, KeySpace_Jetpack,
        KeyLCtrl_Descend, FuelBackground_Jetpack, FuelFill_Jetpack, Background_PushBall, BackgroundInverted_PushBall, Icon_PushBall, IconInverted_PushBall, LeftClick_PushBall,
        Background_CatchBall, BackgroundInverted_CatchBall, Icon_CatchBall, IconInverted_CatchBall, CooldownFill_CatchBall, LeftClick_CatchBall
    }

    private static HudOverlayManager _instance;

    private Text[] _hudTexts;
    private Image[] _hudImages;

    private float _crosshairDefaultScale;
    private float _crosshairTargetScale;

    public void Awake() {
        if (_instance == null) {
            _instance = this;
        }

        _hudTexts = GetComponentsInChildren<Text>();
        _hudImages = GetComponentsInChildren<Image>();

        _crosshairDefaultScale = _hudImages[(int)HUDImage.CrosshairBase].rectTransform.localScale.x;
        _crosshairTargetScale = _hudImages[(int)HUDImage.CrosshairMaxRange].rectTransform.localScale.x;

        ResetAllCooldowns();
    }

    public void ResetAllCooldowns() {
        SetPlayerPushOffCooldown();
        SetDashOffCooldown();
        SetCatchOffCooldown();
        SetJetpackActive();
        UpdateFuel(1f);
        ResetCrosshair();
    }

    public void UpdateGoalCount(HUDText hudText, int goals) {
        _hudTexts[(int)hudText].text = "" + goals;
    }

    public void UpdateCrosshair(float holdingPercentage) {
        _hudImages[(int)HUDImage.CrosshairMaxRange].enabled = true;
        _hudImages[(int)HUDImage.CrosshairOutline].enabled = false;
        _hudImages[(int)HUDImage.CrosshairBase].rectTransform.localScale = new Vector3(_crosshairDefaultScale + (_crosshairTargetScale - _crosshairDefaultScale) * holdingPercentage, _crosshairDefaultScale + (_crosshairTargetScale - _crosshairDefaultScale) * holdingPercentage, 1f);
    }

    public void ResetCrosshair() {
        _hudImages[(int)HUDImage.CrosshairMaxRange].enabled = false;
        _hudImages[(int)HUDImage.CrosshairOutline].enabled = true;
        _hudImages[(int)HUDImage.CrosshairBase].rectTransform.localScale = new Vector3(_crosshairDefaultScale, _crosshairDefaultScale, 1f);
    }

    public void UpdateMatchTimer(float time) {
        string minString = "" + Mathf.FloorToInt(time / 60f);

        int sec = Mathf.FloorToInt(time) % 60;
        string secString = "" + sec;

        //Some formatting
        if (sec < 10) {
            secString = "0" + sec;
        } else if (sec == 0) {
            secString = "00";
        }

        _hudTexts[(int)HUDText.MatchTimer].text = minString + ":" + secString;
    }

    public void UpdateMatchTimer(string message) {
        _hudTexts[(int)HUDText.MatchTimer].text = message;
    }

    public void UpdateRoundCountdown(float countdown) {
        _hudTexts[(int)HUDText.RoundCountdown].text = "" + Mathf.CeilToInt(countdown);

        if (countdown <= 0) {
            _hudTexts[(int)HUDText.RoundCountdown].enabled = false;
        } else {
            _hudTexts[(int)HUDText.RoundCountdown].enabled = true;
        }
    }

    public void SetPlayerPushOnCooldown(float fillAmount, float time) {
        if (!_hudImages[(int)HUDImage.Background_PlayerPush].enabled) {
            _hudImages[(int)HUDImage.Background_PlayerPush].enabled = true;
            _hudImages[(int)HUDImage.Icon_PlayerPush].enabled = true;

            _hudImages[(int)HUDImage.BackgroundInverted_PlayerPush].enabled = false;
            _hudImages[(int)HUDImage.IconInverted_PlayerPush].enabled = false;
        }

        _hudImages[(int)HUDImage.CooldownFill_PlayerPush].fillAmount = fillAmount;
        _hudTexts[(int)HUDText.CooldownText_PlayerPush].text = Mathf.CeilToInt(time) + "";
    }

    public void SetPlayerPushOffCooldown() {
        if (_hudImages[(int)HUDImage.Background_PlayerPush].enabled) {
            _hudImages[(int)HUDImage.Background_PlayerPush].enabled = false;
            _hudImages[(int)HUDImage.Icon_PlayerPush].enabled = false;

            _hudImages[(int)HUDImage.BackgroundInverted_PlayerPush].enabled = true;
            _hudImages[(int)HUDImage.IconInverted_PlayerPush].enabled = true;
        }

        _hudImages[(int)HUDImage.CooldownFill_PlayerPush].fillAmount = 0f;
        _hudTexts[(int)HUDText.CooldownText_PlayerPush].text = "";
    }

    public void SetDashOnCooldown(float fillAmount, float time) {
        if (!_hudImages[(int)HUDImage.Background_Dash].enabled) {
            _hudImages[(int)HUDImage.Background_Dash].enabled = true;
            _hudImages[(int)HUDImage.Icon_Dash].enabled = true;

            _hudImages[(int)HUDImage.BackgroundInverted_Dash].enabled = false;
            _hudImages[(int)HUDImage.IconInverted_Dash].enabled = false;
        }

        _hudImages[(int)HUDImage.CooldownFill_Dash].fillAmount = fillAmount;
        _hudTexts[(int)HUDText.CooldownText_Dash].text = Mathf.Round(time) + "";
    }

    public void SetDashOffCooldown() {
        if (_hudImages[(int)HUDImage.Background_Dash].enabled) {
            _hudImages[(int)HUDImage.Background_Dash].enabled = false;
            _hudImages[(int)HUDImage.Icon_Dash].enabled = false;

            _hudImages[(int)HUDImage.BackgroundInverted_Dash].enabled = true;
            _hudImages[(int)HUDImage.IconInverted_Dash].enabled = true;
        }

        _hudImages[(int)HUDImage.CooldownFill_Dash].fillAmount = 0f;
        _hudTexts[(int)HUDText.CooldownText_Dash].text = "";
    }

    public void SetJetpackActive() {
        _hudImages[(int)HUDImage.Background_Jetpack].enabled = true;

        _hudImages[(int)HUDImage.Icon_Jetpack].enabled = true;
        _hudImages[(int)HUDImage.KeySpace_Jetpack].enabled = true;

        _hudImages[(int)HUDImage.Icon_Descend].enabled = false;
        _hudImages[(int)HUDImage.KeyLCtrl_Descend].enabled = false;
    }

    public void SetDescendActive() {
        _hudImages[(int)HUDImage.Icon_Jetpack].enabled = false;
        _hudImages[(int)HUDImage.KeySpace_Jetpack].enabled = false;

        _hudImages[(int)HUDImage.Icon_Descend].enabled = true;
        _hudImages[(int)HUDImage.KeyLCtrl_Descend].enabled = true;
    }

    public void UpdateFuel(float fill) {
        _hudImages[(int)HUDImage.FuelFill_Jetpack].fillAmount = fill;
    }

    public void SetCatchOnCooldown(float fillAmount, float time) {
        if (!_hudImages[(int)HUDImage.Background_CatchBall].enabled) {
            _hudImages[(int)HUDImage.Background_CatchBall].enabled = true;
            _hudImages[(int)HUDImage.Icon_CatchBall].enabled = true;

            _hudImages[(int)HUDImage.BackgroundInverted_CatchBall].enabled = false;
            _hudImages[(int)HUDImage.IconInverted_CatchBall].enabled = false;
        }

        _hudImages[(int)HUDImage.CooldownFill_CatchBall].fillAmount = fillAmount;
        _hudTexts[(int)HUDText.CooldownText_CatchBall].text = Mathf.Round(time) + "";
    }

    public void SetCatchOffCooldown() {
        if (_hudImages[(int)HUDImage.Background_CatchBall].enabled) {
            _hudImages[(int)HUDImage.Background_CatchBall].enabled = false;
            _hudImages[(int)HUDImage.Icon_CatchBall].enabled = false;

            _hudImages[(int)HUDImage.BackgroundInverted_CatchBall].enabled = true;
            _hudImages[(int)HUDImage.IconInverted_CatchBall].enabled = true;
        }

        _hudImages[(int)HUDImage.CooldownFill_CatchBall].fillAmount = 0f;
        _hudTexts[(int)HUDText.CooldownText_CatchBall].text = "";
    }

    public static HudOverlayManager Instance {
        get { return _instance; }
    }
}
