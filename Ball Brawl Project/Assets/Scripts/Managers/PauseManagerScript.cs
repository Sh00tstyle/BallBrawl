using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PauseManagerScript : NetworkBehaviour {

    [SyncVar]
    private bool _isPaused;

    [SyncVar]
    private bool _blockInput;

    private static PauseManagerScript _instance;

    public override void OnStartServer() {
        _isPaused = false;
        _blockInput = true;
    }

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    [ClientRpc]
    public void RpcSetPause(bool state) {
        _isPaused = state;
        _blockInput = state;
    }

    [Command]
    public void CmdSetBlockInput(bool state) {
        _blockInput = state;
    }

    public bool IsPaused {
        get { return _isPaused; }
    }

    public bool BlockInput {
        get { return _blockInput; }
    }

    public static PauseManagerScript Instance {
        get { return _instance; }
    }
}
