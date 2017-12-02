using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel=2, sendInterval = 0.05f)]
public class BallBehaviourScript : NetworkBehaviour {

    private Rigidbody _ballRb;
    private Collider _ballCollider;
    private MeshRenderer[] _renderers;
    private TrailRenderer _trailRenderer;

    [SerializeField]
    [EventRef]
    public string _impactShield;
    [SerializeField]
    [EventRef]
    public string _ballLoop;

    [SerializeField]
    private Material _ball2RedMat;

    [SerializeField]
    private Material _ball3RedMat;

    [SerializeField]
    private Material _ballTrailRedMat;

    [SerializeField]
    private Material _ball2BlueMat;

    [SerializeField]
    private Material _ball3BlueMat;

    [SerializeField]
    private Material _ballTrailBlueMat;

    [SerializeField]
    private Material _ball2WhiteMat;

    [SerializeField]
    private Material _ball3WhiteMat;

    [SerializeField]
    private Material _ballTrailWhiteMat;

    [SyncVar]
    private bool _isActive;

    [SyncVar]
    private int _lastPlayerID;

    [SyncVar(hook = "AdjustBallColor")] //Hooks the AdjustBallColor() function to the sync var so that it is called whenever the sync var is changed
    private string _currentTeam;

    private static BallBehaviourScript _instance;

    public void Awake() {

        _ballRb = GetComponent<Rigidbody>();
        _ballCollider = GetComponent<Collider>();
        _renderers = GetComponentsInChildren<MeshRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
        AudioManager.PlayEvent(_ballLoop, gameObject, true);

        SetLastPlayerID(0); //Nothing
        ActivateBallBehaviour();

        if(_instance == null) {
            _instance = this;
        }
    }

    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag != Tags.PLAYER) {
            SetLastPlayerID(0);
        }
        AudioManager.PlayOneShot(_impactShield, gameObject);
    }

    private void AdjustBallColor(string newValue) {
        if(newValue == Teams.TEAM_A) {
            _renderers[0].material = _ball2RedMat;
            _renderers[1].material = _ball3RedMat;
            _trailRenderer.material = _ballTrailRedMat;
        } else if(newValue == Teams.TEAM_B) {
            _renderers[0].material = _ball2BlueMat;
            _renderers[1].material = _ball3BlueMat;
            _trailRenderer.material = _ballTrailBlueMat;
        } else {
            _renderers[0].material = _ball2WhiteMat;
            _renderers[1].material = _ball3WhiteMat;
            _trailRenderer.material = _ballTrailWhiteMat;
        }
    }

    private void SetLastPlayerID(int playerID) {
        _lastPlayerID = playerID;
    }

    private void SetCurrentTeam(string team) {
        _currentTeam = team;
    }

    private void SetIsActive(bool isActive) {
        _isActive = isActive;
    }

    public void SetBallPosition(Vector3 newPos) {
        transform.position = newPos;
    }

    public void ResetBall(Vector3 position) {
        transform.position = position;
        _ballRb.velocity = Vector3.zero;

        SetLastPlayerID(0);
        SetCurrentTeam(Teams.TEAM_NEUTRAL);
    }

    private void EnableBall() {
        for(int i = 0; i < _renderers.Length; i++) {
            _renderers[i].enabled = true;
        }

        _ballRb.isKinematic = false;
        _ballRb.useGravity = true;

        _trailRenderer.enabled = true;

        _ballCollider.enabled = true;
    }

    private void DisableBall() {
        for (int i = 0; i < _renderers.Length; i++) {
            _renderers[i].enabled = false;
        }

        _ballRb.isKinematic = true;
        _ballRb.useGravity = false;

        _trailRenderer.enabled = false;

        _ballCollider.enabled = false;
    }

    public void ActivateBallBehaviour() {
        SetIsActive(true);

        EnableBall();

        _ballRb.velocity = Vector3.zero;
    }

    public void DeactivateBallBehaviour(int playerID) {
        SetIsActive(false);

        SetLastPlayerID(playerID);

        DisableBall();

        _ballRb.velocity = Vector3.zero;
    }

    public void PushBall(Vector3 direction, float strength, string playerTeam) {
        _ballRb.AddForce(direction * strength);
        SetCurrentTeam(playerTeam);
    }

    public bool IsActive {
        get { return _isActive; }
    }

    public int LastPlayerID {
        get { return _lastPlayerID; }
    }

    public static BallBehaviourScript Instance {
        get { return _instance; }
    }
}
