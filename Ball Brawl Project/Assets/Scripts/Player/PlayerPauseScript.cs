using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerPauseScript : NetworkBehaviour {

	public void Update () {
        if (PauseManagerScript.Instance.IsPaused) {
            UiManagerScript.Instance.ActivatePauseMenu();
        } else {
            UiManagerScript.Instance.DeactivatePauseMenu();
        }

        if (!isLocalPlayer) return;

        //DEBUG: Change it to something else lateron
        if (Input.GetKeyDown(KeyCode.P) && GameStateManager.Instance.CurrentState == GameStates.STATE_INGAME) {
            if (PauseManagerScript.Instance.IsPaused) {
                CmdSetPause(false);
            } else {
                CmdSetPause(true);
            }
        }
    }

    [Command]
    private void CmdSetPause(bool state) {
        PauseManagerScript.Instance.RpcSetPause(state);
    }
}
