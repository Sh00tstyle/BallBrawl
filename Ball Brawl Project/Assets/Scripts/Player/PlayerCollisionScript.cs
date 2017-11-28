using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCollisionScript : NetworkBehaviour {

    private Vector3 _spawnPos;

    private PlayerInteractionScript _ballInteraction;
    private PlayerControllerRigidbody _playerController;
    private PlayerInitScript _playerInit;
    private CameraController _cameraController;

    public override void OnStartLocalPlayer() {
        _spawnPos = transform.position;
    }

    public void Awake() {
        _ballInteraction = GetComponent<PlayerInteractionScript>();
        _playerController = GetComponent<PlayerControllerRigidbody>();
        _playerInit = GetComponent<PlayerInitScript>();
    }

    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == Tags.BALL) {
            BallBehaviourScript ballBehaviour = collision.gameObject.GetComponent<BallBehaviourScript>();

            if(ballBehaviour.LastPlayerID != _ballInteraction.PlayerID) Respawn();
        }
    }

    private void Respawn() {
        if (!isLocalPlayer) return;

        //restoring original position and rotation
        transform.position = _spawnPos;

        if(_cameraController == null) _cameraController = GetComponentInChildren<CameraController>(); //cant be done in awake, since the camera is enabled after the client connects
        _cameraController.ResetRotation();

        if(_playerInit.AssignedTeam == TeamManager.TEAM_A) {
            _playerController.SetRotationPlayer(180f);
        } else if(_playerInit.AssignedTeam == TeamManager.TEAM_B) {
            _playerController.SetRotationPlayer(0f);
        }
    }
}
