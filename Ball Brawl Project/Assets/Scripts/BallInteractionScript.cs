using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallInteractionScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _playerCamera;

    [SerializeField]
    private Transform _ballParent;

    private BallCollisionScript _ballCollision;
    private BallBehaviourScript _ballBehaviour;

    private bool _isHolding;

    public override void OnStartLocalPlayer() {
        _playerCamera.SetActive(true);

        _ballCollision = GetComponentInChildren<BallCollisionScript>();
    }

    public void Update() {
        if (GameManagerScript.IsPaused || !isLocalPlayer) return;

        ProcessMouseInput();

    }

    private void ProcessMouseInput() {
        if(Input.GetMouseButtonDown(0) && _ballCollision.InRange) {
            CmdPushBall();
        } else if(Input.GetMouseButton(1) && _ballCollision.InRange) {
            CmdHoldBall();
        }
    }

    [Command]
    private void CmdHoldBall() {
        if (_ballBehaviour == null && _ballCollision.Ball != null) _ballBehaviour = _ballCollision.Ball.GetComponent<BallBehaviourScript>();

        Debug.Log("Hold ball");
        _ballBehaviour.DeactivateBallBehaviour();

        Transform ballTrans = _ballCollision.Ball.transform;

        ballTrans.parent = _ballParent;
        ballTrans.localPosition = Vector3.zero;
    }

    [Command]
    private void CmdPushBall() {
        if (_ballBehaviour == null && _ballCollision.Ball != null) _ballBehaviour = _ballCollision.Ball.GetComponent<BallBehaviourScript>();

        Debug.Log("Push ball");
        _ballBehaviour.PushBall(Camera.main.transform.forward, 300f);
    }
}
