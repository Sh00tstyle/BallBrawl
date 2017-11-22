using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManagerScript : NetworkBehaviour {

    private static GameManagerScript _instance = null;

    [SyncVar]
    private bool _isPaused = false;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        } else if(_instance != this) {
            Destroy(gameObject); //In case there is another gamemanager up and running already
        }
    }

    [Command]
    public void CmdPauseGame() {
        Debug.Log("Game paused");
        Time.timeScale = 0f;
        _isPaused = true;
    }

    [Command]
    public void CmdResumeGame() {
        Debug.Log("Game resumed");
        Time.timeScale = 1f;
        _isPaused = false;
    }

    public static bool IsPaused {
        get { return _instance._isPaused; }
    }
}
