using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviourScript : MonoBehaviour {

    private Rigidbody _ballRb;
    private Collider _ballCollider;
    private bool _isActive;

    public void Awake() {
        _ballRb = GetComponent<Rigidbody>();
        _ballCollider = GetComponent<Collider>();

        ActivateBallBehaviour();
    }

    public void ActivateBallBehaviour() {
        if (_isActive) return;

        _ballRb.isKinematic = false;
        _ballRb.useGravity = true;

        _ballCollider.enabled = true;

        _isActive = true;
    }

    public void DeactivateBallBehaviour() {
        if (!_isActive) return;

        _ballRb.isKinematic = true;
        _ballRb.useGravity = false;

        _ballCollider.enabled = false;

        _ballRb.velocity = Vector3.zero;
        _isActive = false;
    }

    public void PushBall(Vector3 direction, float strength) {
        _ballRb.AddRelativeForce(direction * strength);
    }

    public bool IsActive {
        get { return _isActive; }
    }
}
