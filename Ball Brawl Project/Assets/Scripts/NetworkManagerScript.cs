using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerScript : NetworkBehaviour {

    public GameObject ballPrefab;

    public override void OnStartServer() {
        GameObject ball = Instantiate(ballPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);
        NetworkServer.Spawn(ball);
    }
}
