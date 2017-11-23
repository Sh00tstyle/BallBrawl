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

    private void Awake() {
        _ballBehaviour = BallBehaviourScript.Instance;
    }

    public void Update() {
        if(!isLocalPlayer) return;
        //if (GameManagerScript.IsPaused) return;

        ProcessMouseInput();

        if(_isHolding) {
            HudOverlayManager.Instance.UpdateHoldingBar(_holdingTimer / _maxHoldingTime);
            CmdHoverBall();
        }

        _holdingTimer += Time.deltaTime;
    }

    private void ProcessMouseInput() {
        if (_isHolding && (Input.GetMouseButtonDown(0) || _holdingTimer > _maxHoldingTime)) {
            _isHolding = false;
            HudOverlayManager.Instance.UpdateHoldingBar(0f);
            CmdThrowBall();
        } else if (_ballCollision.InRange) {
            if (Input.GetMouseButtonDown(1)) {
                CmdPushBall();
            } else if(Input.GetMouseButtonDown(0) && !_isHolding) {
                _isHolding = true;
                _holdingTimer = 0f;
                CmdCatchBall();
            }
        }      
    }

    [Command]
    private void CmdHoverBall() {
        _ballBehaviour.SetBallPosition(_ballParent.position);
    }

    [Command]
    private void CmdCatchBall() {
        _ballBehaviour.DeactivateBallBehaviour();
    }

    [Command]
    private void CmdThrowBall() {
        _ballBehaviour.ActivateBallBehaviour();

        Vector3 throwingDir = _playerCamera.transform.forward;
        throwingDir = Quaternion.Euler(new Vector3(Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor), 
            Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor), 0)) * throwingDir; //rotating randomly based on the holding time

        _ballBehaviour.PushBall(throwingDir, _throwingForce);
    }

    [Command]
    private void CmdPushBall() {
        _ballBehaviour.PushBall(_playerCamera.transform.forward, _throwingForce);
    }
}
