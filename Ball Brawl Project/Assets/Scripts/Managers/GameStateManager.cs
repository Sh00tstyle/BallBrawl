using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameStateManager : NetworkBehaviour {

    private static GameStateManager _instance;

    [SerializeField]
    private Transform _ballSpawnPos;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    [Command]
    public void CmdResetRound() {
        BallBehaviourScript.Instance.ResetBall(_ballSpawnPos.position);

        for(int i = 0; i < PlayerManager.Instance.PlayerCount; i++) {
            //Respawn each player
            PlayerObject playerObj = PlayerManager.Instance.GetPlayerAt(i);
            playerObj.playerObject.GetComponent<PlayerCollisionScript>().RpcRespawn();
        }
    }

    public static GameStateManager Instance {
        get { return _instance; }
    }
}
