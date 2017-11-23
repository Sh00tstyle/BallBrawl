using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallBehaviourScript : NetworkBehaviour {

    private Rigidbody _ballRb;
    private Collider _ballCollider;

    private static BallBehaviourScript _instance;

    public void Awake() {
        _ballRb = GetComponent<Rigidbody>();
        _ballCollider = GetComponent<Collider>();

        ActivateBallBehaviour();

        if(_instance == null) {
            _instance = this;
        }
    }

    public void SetBallPosition(Vector3 newPos) {
        transform.position = newPos;
    }

    public void ActivateBallBehaviour() {
        Debug.Log("Activating Ball");

        _ballRb.isKinematic = false;
        _ballRb.useGravity = true;

        _ballCollider.enabled = true;

        _ballRb.velocity = Vector3.zero;
    }

    public void DeactivateBallBehaviour() {
        Debug.Log("Deactivating Ball");

        _ballRb.isKinematic = true;
        _ballRb.useGravity = false;

        _ballCollider.enabled = false;

        _ballRb.velocity = Vector3.zero;
    }

    public void PushBall(Vector3 direction, float strength) {
        //_ballRb.velocity = Vector3.zero;
        _ballRb.AddForce(direction * strength);
    }

    public static BallBehaviourScript Instance {
        get { return _instance; }
    }
}
