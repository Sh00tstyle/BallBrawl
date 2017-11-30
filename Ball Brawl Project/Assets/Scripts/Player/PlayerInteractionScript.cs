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
    private GameObject _visualBall;

    [SerializeField]
    private Transform _ballParent;

    [SerializeField]
    private float _throwingForce;

    [SerializeField]
    private float _pushForce;

    [SerializeField]
    private float _maxHoldingTime;

    [SerializeField]
    private float _abilityCooldown;

    [SerializeField]
    private float _precisionReductionFactor;

    [SyncVar]
    private bool _isHolding;

    private InteractionRangeScript _interactionRange;
    private BallBehaviourScript _ballBehaviour;
    private PlayerIdScript _playerId;
    private PlayerTeamScript _playerTeam;

    private float _holdingTimer;
    private float _catchCooldownTimer;
    private float _abilityCooldownTimer;

    public override void OnStartLocalPlayer() {
        _playerCamera.SetActive(true);

        _interactionRange = GetComponentInChildren<InteractionRangeScript>();
    }

    private void Awake() {
        _ballBehaviour = BallBehaviourScript.Instance;
        _playerId = GetComponent<PlayerIdScript>();
        _playerTeam = GetComponent<PlayerTeamScript>();
    }

    public void Update() {
        if (!isLocalPlayer && _isHolding) _visualBall.SetActive(true);
        else if (!isLocalPlayer && !_isHolding) _visualBall.SetActive(false);

        if(!isLocalPlayer) return;
        if (PauseManagerScript.Instance.IsPaused || PauseManagerScript.Instance.BlockInput) return;

        ProcessMouseInput();

        if(_isHolding) {
            HudOverlayManager.Instance.UpdateHoldingBar(_holdingTimer / _maxHoldingTime);
        } else {
            HudOverlayManager.Instance.UpdateHoldingBar(0f);
        }

        _holdingTimer += Time.deltaTime;
        _catchCooldownTimer += Time.deltaTime;
        _abilityCooldownTimer += Time.deltaTime;
    }

    private void ProcessMouseInput() {
        if (_isHolding && (Input.GetMouseButtonUp(1) || _holdingTimer > _maxHoldingTime)) {
            _catchCooldownTimer = 0f;
            HudOverlayManager.Instance.UpdateHoldingBar(0f);

            _localBall.SetActive(false);

            //Calculating the direction the ball gets push towards
            Vector3 throwingDir = _playerCamera.transform.forward;
            throwingDir = Quaternion.Euler(new Vector3(Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor),
                Random.Range(-_holdingTimer * _precisionReductionFactor, _holdingTimer * _precisionReductionFactor), 0)) * throwingDir; //rotating randomly based on the holding time

            CmdSetIsHolding(false);
            CmdThrowBall(throwingDir);
        } else if (_interactionRange.BallInRange) {
            if (Input.GetMouseButtonDown(0)) {
                CmdPushBall();
            } else if(Input.GetMouseButton(1) && !_isHolding && _catchCooldownTimer >= 2f) {
                _holdingTimer = 0f;
                _localBall.SetActive(true);

                CmdSetIsHolding(true);
                CmdCatchBall();
            }
        }

        if(Input.GetKeyDown(KeyCode.Q) && _interactionRange.PlayerInRange && _abilityCooldownTimer >= _abilityCooldown) {
            Debug.Log("Pushing player");

            //Pushed player releases the ball if he holds it
            PlayerInteractionScript playerInteraction = _interactionRange.PlayerObject.GetComponent<PlayerInteractionScript>();
            if (playerInteraction.IsHolding) {
                playerInteraction.CmdReleaseBall();
                playerInteraction.CmdSetIsHolding(false);
            }

            Vector3 deltaVec = _interactionRange.PlayerObject.transform.position - transform.position;

            CmdPushPlayer(_interactionRange.PlayerObject, deltaVec.normalized, _pushForce);

            _abilityCooldownTimer = 0f;
        }
    }

    [Command(channel = 2)]
    private void CmdPushPlayer (GameObject pTarget, Vector3 direction, float force) {
        //Requests the target to push itself into the target direction
        pTarget.GetComponent<PlayerControllerRigidbody>().RpcReceivePush(direction, force);
    }

    [Command(channel = 2)]
    private void CmdSetIsHolding(bool state) {
        _isHolding = state;
    }

    [Command(channel = 2)]
    private void CmdCatchBall() {
        _ballBehaviour.DeactivateBallBehaviour(_playerId.ID);

        _ballBehaviour.SetBallPosition(new Vector3(1000f, 1000f, 1000f)); //somewhere outside of the playingfield
    }

    [Command(channel = 2)]
    private void CmdThrowBall(Vector3 throwingDir) {
        _ballBehaviour.ActivateBallBehaviour();
        _ballBehaviour.SetBallPosition(_ballParent.position); 

        _ballBehaviour.PushBall(throwingDir, _throwingForce, _playerTeam.AssignedTeam);
    }

    [Command(channel = 2)]
    private void CmdPushBall() {
        _ballBehaviour.PushBall(_playerCamera.transform.forward, _throwingForce, _playerTeam.AssignedTeam);
    }

    [Command(channel = 2)]
    public void CmdReleaseBall() {
        //Activating the ball and dropping it in front of the player
        _ballBehaviour.ActivateBallBehaviour();
        _ballBehaviour.SetBallPosition(_ballParent.position);
    }

    public void ResetCooldowns() {
        _abilityCooldownTimer = 0f;
    }

    public bool IsHolding {
        get { return _isHolding; }
    }

    public float AbilityCooldownTimer {
        get { return _abilityCooldownTimer; }
    }

    public GameObject VisualBall {
        get { return _visualBall; }
    }

    public GameObject LocalBall {
        get { return _localBall; }
    }
}
