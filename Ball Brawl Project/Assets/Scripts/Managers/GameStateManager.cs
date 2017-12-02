using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2)]
public class GameStateManager : NetworkBehaviour {

    private static GameStateManager _instance;

    [SerializeField]
    private Transform _ballSpawnPos;

    [SerializeField]
    private float _slowdownDuration;

    [SyncVar]
    private int _currentState;

    [SyncVar]
    private float _timeScale;

    [SyncVar]
    private float _matchTimer;

    [SyncVar]
    private float _matchStartTimer;

    private float _slowdownTimer;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
            _timeScale = 1f;
            _matchTimer = 300f; //5 minutes
        }
    }

    public void Update() {
        if (PauseManagerScript.Instance.IsPaused) return;

        if (Input.GetKeyDown(KeyCode.M)) {
            //DEBUG: Initilizes the round countdown
            CmdSetState(GameStates.STATE_READYROUND);
        }

        UpdateState();

        _matchStartTimer -= Time.deltaTime;
        _slowdownTimer -= Time.deltaTime;

        HudOverlayManager.Instance.UpdateRoundCountdown(_matchStartTimer);
    }

    private void UpdateState() {
        switch (_currentState) {
            case GameStates.STATE_PAUSE:
                break;

            case GameStates.STATE_READYROUND:
                if (_matchStartTimer <= 0) {
                    CmdSetState(GameStates.STATE_INGAME);
                }
                break;

            case GameStates.STATE_INGAME:
                _matchTimer -= Time.deltaTime;
                HudOverlayManager.Instance.UpdateMatchTimer(_matchTimer);

                if(_matchTimer <= 0f) {
                    //TODO
                    Debug.Log("Match over");
                }
                break;

            case GameStates.STATE_SLOWDOWN:
                if (_timeScale > 0.1f) _timeScale -= Time.deltaTime * 1.5f;

                if(_slowdownTimer <= 0) CmdSetState(GameStates.STATE_READYROUND);
                break;

            default:
                break;
        }
    }
    
    [Command]
    public void CmdSetState(int state) {
        _currentState = state;

        switch(_currentState) {
            case GameStates.STATE_PAUSE:
                PauseManagerScript.Instance.CmdSetPause(true);
                break;

            case GameStates.STATE_READYROUND:
                _timeScale = 1f;

                PauseManagerScript.Instance.CmdSetBlockInput(true);

                CmdResetBall();
                CmdResetPlayers();

                _matchStartTimer = 3f;
                break;

            case GameStates.STATE_INGAME:
                CmdResetPlayers();

                PauseManagerScript.Instance.CmdSetPause(false); //Disables both pause and input blocking
                break;

            case GameStates.STATE_SLOWDOWN:
                _slowdownTimer = _slowdownDuration;
                break;

            default:
                break;
        }
    }

    [Command]
    private void CmdResetBall() {
        BallBehaviourScript.Instance.ResetBall(_ballSpawnPos.position);
    }

    [Command]
    private void CmdResetPlayers() {
        for (int i = 0; i < PlayerManager.Instance.PlayerCount; i++) {
            //Respawn each player
            PlayerObject playerObj = PlayerManager.Instance.GetPlayerAt(i);
            playerObj.playerObject.GetComponent<PlayerCollisionScript>().RpcRespawn();
            playerObj.playerObject.GetComponent<PlayerInteractionScript>().ResetCooldowns();

            PlayerControllerRigidbody playerController = playerObj.playerObject.GetComponent<PlayerControllerRigidbody>();
            playerController.RpcResetVelocity();
            playerController.ResetCooldowns();
        }
    } 

    public static GameStateManager Instance {
        get { return _instance; }
    }
}
