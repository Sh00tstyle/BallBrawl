using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FakeBallScript : NetworkBehaviour {

    public void OnTriggerEnter(Collider other) {
        if (other.tag == Tags.BALL) return;

        CmdSetBallPos();
        Destroy(gameObject);
    }

    public void OnDestroy() {
        CmdSetBallPos();
    }

    [Command (channel = 2)]
    public void CmdSetBallPos() {
        BallBehaviourScript.Instance.SetBallPosition(transform);
    }
}
