using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PauseManagerScript : NetworkBehaviour {

    [SyncVar(hook = "AdjustTimeScale")]
    private bool _isPaused;

    private static PauseManagerScript _instance;

    public override void OnStartServer() {
        _isPaused = false;
    }

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    [Command]
    public void CmdPauseGame() {
        _isPaused = true;
    }

    [Command]
    public void CmdResumeGame() {
        _isPaused = false;
    }

    private void AdjustTimeScale(bool newValue) {
        if (newValue) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public bool IsPaused {
        get { return _isPaused; }
    }

    public static PauseManagerScript Instance {
        get { return _instance; }
    }
}
