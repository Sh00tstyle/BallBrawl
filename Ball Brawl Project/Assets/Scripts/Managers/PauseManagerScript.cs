using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PauseManagerScript : NetworkBehaviour {

    [SyncVar]
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
        _isPaused = false;
        Time.timeScale = 0f;
    }

    [Command]
    public void CmdResumeGame() {
        _isPaused = true;
        Time.timeScale = 1f;
    }

    public bool IsPaused {
        get { return _isPaused; }
    }

    public static PauseManagerScript Instance {
        get { return _instance; }
    }
}
