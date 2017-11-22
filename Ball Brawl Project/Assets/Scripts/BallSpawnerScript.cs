using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallSpawnerScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _ballPrefab;

    [SerializeField]
    private Transform _spawnPos;

    public override void OnStartServer() {
        GameObject ball = Instantiate(_ballPrefab, _spawnPos.position, Quaternion.identity);
        NetworkServer.Spawn(ball);
    }
}
