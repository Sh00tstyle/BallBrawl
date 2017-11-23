using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallInteractionScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _playerCamera;

    [SerializeField]
    private Transform _ballParent;

    [SerializeField]
    private float _throwingForce;

    [SerializeField]
    private float _maxHoldingTime;

    [SerializeField]
    private float _precisionReductionFactor;

    private BallCollisionScript _ballCollision;
    private BallBehaviourScript _ballBehaviour;

    private float _holdingTimer;

    private bool _isHolding;

    public override void OnStartLocalPlayer() {
        _playerCamera.SetActive(true);

        _ballCollision = GetComponentInChildren<BallCollisionScript>();
    }

    public void Update() {
        if (GameManagerScript.IsPaused || !isLocalPlayer) return;

        ProcessMouseInput();

        if(_isHolding) {
            HudOverlayManager.Instance.UpdateHoldingBar(_holdingTimer / _maxHoldingTime);
        }
    }

    private void ProcessMouseInput() {
        if (_isHolding && (Input.GetMouseButtonDown(0) || _holdingTimer > _maxHoldingTime)) {
            CmdThrowBall();
        } else if (_ballCollision.InRange) {
            if (_ballBehaviour == null && _ballCollision.Ball != null) _ballBehaviour = _ballCollision.Ball.GetComponent<BallBehaviourScript>();

            if (Input.GetMouseButtonDown(1)) {
                CmdPushBall();
            } else if(Input.GetMouseButtonDown(0) && !_isHolding) {
                CmdCatchBall();
            }
        }

        _holdingTimer += Time.deltaTime;
    }

    [Command]
    private void CmdCatchBall() {
        _isHolding = true;
        _ballBehaviour.DeactivateBallBehaviour();

        Transform ballTrans = _ballCollision.Ball.transform;

        ballTrans.parent = _ballParent;
        ballTrans.localPosition = Vector3.zero;

        _holdingTimer = 0f;
    }

    [Command]
    private void CmdThrowBall() {
        Debug.Log("Throw");
        _isHolding = false;
        _ballBehaviour.ActivateBallBehaviour();

        Transform ballTrans = _ballCollision.Ball.transform;

        ballTrans.parent = null; //no parent

        //Set fill bar to 0/empty
        HudOverlayManager.Instance.UpdateHoldingBar(0f);

        Vector3 throwingDir = Camera.main.transform.forward;
        throwingDir = Quaternion.Euler(new Vector3(Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor), 
            Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor), 0)) * throwingDir; //rotating randomly based on the holding time

        _ballBehaviour.PushBall(throwingDir, _throwingForce);
    }

    [Command]
    private void CmdPushBall() {
        Debug.Log("Push");

        _ballBehaviour.PushBall(Camera.main.transform.forward, _throwingForce);
    }
}
