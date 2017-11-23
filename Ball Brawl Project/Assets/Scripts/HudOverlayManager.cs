using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudOverlayManager : MonoBehaviour {

	public enum HUDText { SimpleCrosshair, CaptionTeamA, CaptionTeamB, CounterTeamA, CounterTeamB }
    public enum HUDImage { HoldingBarEmpty, HoldingBarFilled }

    private static HudOverlayManager _instance = null;

    private Text[] _hudTexts;
    private Image[] _hudImages;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(gameObject); //To ensure we only have one
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

    public static HudOverlayManager Instance {
        get { return _instance; }
    }
}
