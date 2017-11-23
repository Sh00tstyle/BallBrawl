using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCollisionScript : NetworkBehaviour {

    [SyncVar]
    private Vector3 _spawnPos;

    [SyncVar]
    private Vector3 _spawnRotation;

    public override void OnStartLocalPlayer() {
        _spawnPos = transform.position;
        _spawnRotation = transform.eulerAngles;
    }

    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == Tags.BALL) {
            Debug.Log("You died, resetting position");

            RpcRespawn();
        }
    }

    [ClientRpc]
    private void RpcRespawn() {
        if (!isLocalPlayer) return;

        transform.position = _spawnPos;
        transform.eulerAngles = _spawnRotation;
    }
}
