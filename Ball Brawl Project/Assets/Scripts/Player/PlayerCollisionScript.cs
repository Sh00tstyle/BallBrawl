using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCollisionScript : NetworkBehaviour {

    private Vector3 _spawnPos;

    private PlayerControllerRigidbody _playerController;
    private PlayerTeamScript _playerTeam;
    private CameraController _cameraController;

    public override void OnStartLocalPlayer() {
        _spawnPos = transform.position;
    }

    public void Awake() {
        _playerController = GetComponent<PlayerControllerRigidbody>();
        _playerTeam = GetComponent<PlayerTeamScript>();
    }

    public void ResetPlayer() {
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
        ResetPlayer();
    }
}
