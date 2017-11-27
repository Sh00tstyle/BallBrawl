using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel=2, sendInterval = 0.05f)]
public class BallBehaviourScript : NetworkBehaviour {

    private Rigidbody _ballRb;
    private Collider _ballCollider;
    private MeshRenderer _renderer;

    [SyncVar]
    private bool _isActive;

    [SyncVar]
    private int _lastPlayerID;

    private static BallBehaviourScript _instance;

    public void Awake() {
        _ballRb = GetComponent<Rigidbody>();
        _ballCollider = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();

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
    }

    public void Update() {
        //DEBUG: Only to test/ensure that the ball is in the same state at both sides
        //if (_isActive) EnableBall();
        //else DisableBall();
    }

    private void SetLastPlayerID(int playerID) {
        _lastPlayerID = playerID;
    }

    private void SetIsActive(bool isActive) {
        _isActive = isActive;
    }

    public void SetBallPosition(Vector3 newPos) {
        transform.position = newPos;
    }

    private void EnableBall() {
        _renderer.enabled = true;

        _ballRb.isKinematic = false;
        _ballRb.useGravity = true;

        _ballCollider.enabled = true;
    }

    private void DisableBall() {
        _renderer.enabled = false;

        _ballRb.isKinematic = true;
        _ballRb.useGravity = false;

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

    public void PushBall(Vector3 direction, float strength) {
        //_ballRb.velocity = Vector3.zero;
        _ballRb.AddForce(direction * strength);
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
