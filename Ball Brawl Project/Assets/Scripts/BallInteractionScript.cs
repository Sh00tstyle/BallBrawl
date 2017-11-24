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
    private float _catchCooldown;

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

        ProcessMouseInput();

        if(_isHolding) {
            HudOverlayManager.Instance.UpdateHoldingBar(_holdingTimer / _maxHoldingTime);
        }

        _holdingTimer += Time.deltaTime;
        _catchCooldown += Time.deltaTime;
    }

    private void ProcessMouseInput() {
        if (_isHolding && (Input.GetMouseButtonUp(1) || _holdingTimer > _maxHoldingTime)) {
            _isHolding = false;
            _catchCooldown = 0f;
            HudOverlayManager.Instance.UpdateHoldingBar(0f);

            CmdThrowBall();
        } else if (_ballCollision.InRange) {
            if (Input.GetMouseButtonDown(0)) {
                CmdPushBall();
            } else if(Input.GetMouseButton(1) && !_isHolding && _catchCooldown >= 2f) {
                _isHolding = true;
                _holdingTimer = 0f;

                CmdCatchBall();
            }
        }      
    }

    [Command]
    private void CmdCatchBall() {
        _ballBehaviour.SetBallPosition(_ballParent);

        _ballBehaviour.DeactivateBallBehaviour();
    }

    [Command]
    private void CmdThrowBall() {
        _ballBehaviour.SetBallPosition(null);

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
