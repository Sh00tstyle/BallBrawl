using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCollisionScript : NetworkBehaviour {

    private Vector3 _spawnPos;

    private PlayerInteractionScript _ballInteraction;

    public override void OnStartLocalPlayer() {
        _spawnPos = transform.position;
    }

    public void Awake() {
        _ballInteraction = GetComponent<PlayerInteractionScript>();
    }

    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == Tags.BALL) {
            BallBehaviourScript ballBehaviour = collision.gameObject.GetComponent<BallBehaviourScript>();

            if(ballBehaviour.LastPlayerID != _ballInteraction.PlayerID) Respawn();
        }
    }

    private void Respawn() {
        if (!isLocalPlayer) return;

        transform.position = _spawnPos;
    }
}
