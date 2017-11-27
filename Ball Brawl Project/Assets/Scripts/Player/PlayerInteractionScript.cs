using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInteractionScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _playerCamera;

    [SerializeField]
    private GameObject _localBall;

    [SerializeField]
    private Transform _ballParent;

    [SerializeField]
    private float _throwingForce;

    [SerializeField]
    private float _maxHoldingTime;

    [SerializeField]
    private float _precisionReductionFactor;

    [SyncVar]
    private int _playerID;

    private BallCollisionScript _ballCollision;
    private BallBehaviourScript _ballBehaviour;

    private float _holdingTimer;
    private float _catchCooldown;

    private bool _isHolding;

    public override void OnStartLocalPlayer() {
        _playerCamera.SetActive(true);

        _ballCollision = GetComponentInChildren<BallCollisionScript>();

        CmdSetPlayerID(IDManager.Instance.GetNextID());
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

            _localBall.SetActive(false);

            //Calculating the direction the ball gets push towards
            Vector3 throwingDir = _playerCamera.transform.forward;
            throwingDir = Quaternion.Euler(new Vector3(Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor),
                Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor), 0)) * throwingDir; //rotating randomly based on the holding time

            CmdThrowBall(throwingDir);
        } else if (_ballCollision.InRange) {
            if (Input.GetMouseButtonDown(0)) {
                CmdPushBall();
            } else if(Input.GetMouseButton(1) && !_isHolding && _catchCooldown >= 2f) {
                _isHolding = true;
                _holdingTimer = 0f;

                _localBall.SetActive(true);
                CmdCatchBall();
            }
        }      
    }

    [Command]
    private void CmdSetPlayerID(int playerID) {
        _playerID = playerID;
    }

    [Command(channel = 2)]
    private void CmdCatchBall() {
        _ballBehaviour.DeactivateBallBehaviour(_playerID);

        _ballBehaviour.SetBallPosition(new Vector3(1000f, 1000f, 1000f)); //somewhere outside of the playingfield
    }

    [Command(channel = 2)]
    private void CmdThrowBall(Vector3 throwingDir) {
        _ballBehaviour.ActivateBallBehaviour();
        _ballBehaviour.SetBallPosition(_ballParent.position); 

        _ballBehaviour.PushBall(throwingDir, _throwingForce);
    }

    [Command(channel =2)]
    private void CmdPushBall() {
        _ballBehaviour.PushBall(_playerCamera.transform.forward, _throwingForce);
    }

    public bool IsHolding {
        get { return _isHolding; }
    }

    public int PlayerID {
        get { return _playerID; }
    }
}
