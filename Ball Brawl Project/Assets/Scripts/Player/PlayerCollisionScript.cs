using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCollisionScript : NetworkBehaviour {

    private Vector3 _spawnPos;

    private PlayerIdScript _playerId;
    private PlayerControllerRigidbody _playerController;
    private PlayerInteractionScript _playerInteraction;
    private PlayerTeamScript _playerTeam;
    private CameraController _cameraController;

    public override void OnStartLocalPlayer() {
        _spawnPos = transform.position;
    }

    public void Awake() {
        _playerId = GetComponent<PlayerIdScript>();
        _playerController = GetComponent<PlayerControllerRigidbody>();
        _playerInteraction = GetComponent<PlayerInteractionScript>();
        _playerTeam = GetComponent<PlayerTeamScript>();
    }

    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == Tags.BALL) {
            BallBehaviourScript ballBehaviour = collision.gameObject.GetComponent<BallBehaviourScript>();

            if (ballBehaviour.LastPlayerID != _playerId.ID) {
                _playerInteraction.CmdReleaseBall(); //Dropping the ball when you get killed
                Respawn();
            }
        }
    }

    public void Respawn() {
        //restoring original position and rotation
        transform.position = _spawnPos;

        if(_cameraController == null) _cameraController = GetComponentInChildren<CameraController>(); //cant be done in awake, since the camera is enabled after the client connects
        if(_cameraController != null) _cameraController.ResetRotation();

        if(_playerTeam.AssignedTeam == Teams.TEAM_RED) {
            _playerController.SetRotationPlayer(180f);
        } else if(_playerTeam.AssignedTeam == Teams.TEAM_BLUE) {
            _playerController.SetRotationPlayer(0f);
        }
    }

    [ClientRpc]
    public void RpcRespawn() {
        Respawn();
    }
}
