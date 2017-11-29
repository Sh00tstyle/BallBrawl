using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel=2, sendInterval = 0.05f)]
public class BallBehaviourScript : NetworkBehaviour {

    private Rigidbody _ballRb;
    private Collider _ballCollider;
    private MeshRenderer[] _renderers;

    [SyncVar]
    private bool _isActive;

    [SyncVar]
    private int _lastPlayerID;

    private static BallBehaviourScript _instance;

    public void Awake() {
        _ballRb = GetComponent<Rigidbody>();
        _ballCollider = GetComponent<Collider>();
        _renderers = GetComponentsInChildren<MeshRenderer>();

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

    private void SetLastPlayerID(int playerID) {
        _lastPlayerID = playerID;
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
    }

    private void EnableBall() {
        for(int i = 0; i < _renderers.Length; i++) {
            _renderers[i].enabled = true;
        }

        _ballRb.isKinematic = false;
        _ballRb.useGravity = true;

        _ballCollider.enabled = true;
    }

    private void DisableBall() {
        for (int i = 0; i < _renderers.Length; i++) {
            _renderers[i].enabled = false;
        }

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
