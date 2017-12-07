using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2)]
public class GameStateManager : NetworkBehaviour {

    private static GameStateManager _instance;

    [SerializeField]
    private GameObject _roundCountdownPrefab;

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

    [SyncVar]
    private bool _matchEnded;

    private float _slowdownTimer;

    private float _gameResetTimer;

    public override void OnStartServer() {
        _currentState = GameStates.STATE_IDLE;
        _timeScale = 1f;
    }

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        if(HudOverlayManager.Instance != null && _currentState == GameStates.STATE_READYROUND) HudOverlayManager.Instance.UpdateRoundCountdown(_matchStartTimer);
    }

    public void Start() {
        if (_currentState == GameStates.STATE_IDLE) HudOverlayManager.Instance.UpdateMatchTimer("Waiting...");
        else if (_currentState == GameStates.STATE_READYROUND) HudOverlayManager.Instance.UpdateRoundCountdown(_matchStartTimer);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            //DEBUG: Initializes the round countdown
            CmdResetMatch();
        }

        if(Input.GetKeyDown(KeyCode.T)) {
            _matchTimer = 5f;
        }

        UpdateState();

        if (PauseManagerScript.Instance.IsPaused) return;

        Time.timeScale = _timeScale;

        _matchStartTimer -= Time.deltaTime;
        _slowdownTimer -= Time.deltaTime;
        _gameResetTimer -= Time.deltaTime;

        HudOverlayManager.Instance.UpdateRoundCountdown(_matchStartTimer);
    }

    private void UpdateState() {
        switch (_currentState) {
            case GameStates.STATE_IDLE:
                break;

            case GameStates.STATE_READYROUND:
                if (_matchStartTimer <= 0) {
                    CmdSetState(GameStates.STATE_INGAME);
                    AudioManager.SetCrowd(Teams.TEAM_RED, 0.5f);
                    AudioManager.SetCrowd(Teams.TEAM_BLUE, 0.5f);
                }

                HudOverlayManager.Instance.UpdateMatchTimer(_matchTimer);
                break;

            case GameStates.STATE_INGAME:
                _matchTimer -= Time.deltaTime;
                HudOverlayManager.Instance.UpdateMatchTimer(_matchTimer);

                if(_matchTimer <= 0f) {
                    CmdSetState(GameStates.STATE_MATCHEND);
                }
                break;

            case GameStates.STATE_SLOWDOWN:
                if (_timeScale > 0.5f) _timeScale -= Time.deltaTime;

                if(_slowdownTimer <= 0) CmdSetState(GameStates.STATE_READYROUND);
                break;

            case GameStates.STATE_MATCHEND:
                _matchTimer = 0f;
                HudOverlayManager.Instance.UpdateMatchTimer(_matchTimer);

                if (_gameResetTimer <= 0f) {
                    CmdResetMatch();
                }
                break;

            default:
                break;
        }
    }

    [Command]
    public void CmdSetState(int state) {
        _currentState = state;

        switch (_currentState) {
            case GameStates.STATE_IDLE:
                //This will be the "waiting" state when e.g. a player disconnect or not enough players are connected
                PauseManagerScript.Instance.RpcSetPause(false);
                PauseManagerScript.Instance.CmdSetBlockInput(true);

                HudOverlayManager.Instance.UpdateMatchTimer("Waiting...");
                break;

            case GameStates.STATE_READYROUND:
                _timeScale = 1f;

                if (isServer) {
                    GameObject goalImpact = Instantiate(_roundCountdownPrefab, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(goalImpact);
                    Destroy(goalImpact, 4f);
                }

                PauseManagerScript.Instance.CmdSetBlockInput(true);
                CmdResetBall();
                CmdResetPlayers();

                _matchStartTimer = 3f;

                HudOverlayManager.Instance.UpdateMatchTimer(_matchTimer);
                break;

            case GameStates.STATE_INGAME:
                CmdReleaseBall();

                PauseManagerScript.Instance.RpcSetPause(false); //Disables both pause and input blocking
                break;

            case GameStates.STATE_SLOWDOWN:
                _slowdownTimer = _slowdownDuration;
                break;

            case GameStates.STATE_MATCHEND:
                PauseManagerScript.Instance.CmdSetBlockInput(true);
                _matchEnded = true;
                _gameResetTimer = 5f;
                break;

            default:
                break;
        }
    }

    [Command]
    public void CmdResetMatch() {
        _matchTimer = 300f;
        _matchEnded = false;
        GoalSpawnerScript.Instance.ResetGoals();

        HudOverlayManager.Instance.UpdateMatchTimer(_matchTimer);
        CmdSetState(GameStates.STATE_READYROUND);
    }

    [Command]
    private void CmdResetBall() {
        BallBehaviourScript.Instance.ResetBall(_ballSpawnPos.position);
    }

    [Command]
    private void CmdReleaseBall() {
        BallBehaviourScript.Instance.ReleaseBall(25f);
    }

    [Command]
    private void CmdResetPlayers() {
        for (int i = 0; i < PlayerManager.Instance.PlayerCount; i++) {
            //Respawn each player
            PlayerObject playerObj = PlayerManager.Instance.GetPlayerAt(i);
            playerObj.playerObject.GetComponent<PlayerCollisionScript>().RpcRespawn();
            playerObj.playerObject.GetComponent<PlayerInteractionScript>().RpcResetCooldowns();  

            PlayerControllerRigidbody playerController = playerObj.playerObject.GetComponent<PlayerControllerRigidbody>();
            playerController.RpcResetVelocity();
            playerController.RpcResetCooldowns();
        }
    } 

    public int CurrentState {
        get { return _currentState; }
    }

    public bool MatchEnded {
        get { return _matchEnded; }
    }

    public static GameStateManager Instance {
        get { return _instance; }
    }
}
