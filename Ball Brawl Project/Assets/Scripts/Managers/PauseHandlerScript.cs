using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PauseHandlerScript : NetworkBehaviour {

	public void Update () {
        if (PauseManagerScript.Instance.IsPaused) {
            UiManagerScript.Instance.ActivatePauseMenu();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            UiManagerScript.Instance.DeactivatePauseMenu();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.P) && GameStateManager.Instance.CurrentState == GameStates.STATE_INGAME && !PauseManagerScript.Instance.IsPaused) {
            CmdSetPause(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (GameStateManager.Instance.MatchEnded) {
            ResolutionScreenManager.Instance.DisplayResolutionText(GameStateManager.Instance.Winner);
            HudOverlayManager.Instance.UpdateMatchTimer(0f);
            HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamRed, 0);
            HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamBlue, 0);
        } else {
            ResolutionScreenManager.Instance.DisableResolutionTexts();
        }
    }

    public void Resume() {
        CmdSetPause(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [Command]
    public void CmdSetPause(bool state) {
        PauseManagerScript.Instance.RpcSetPause(state);
    }
}
