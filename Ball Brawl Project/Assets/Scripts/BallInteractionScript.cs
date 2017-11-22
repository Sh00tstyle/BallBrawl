using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallInteractionScript : NetworkBehaviour {

    public void Update() {
        if (!isLocalPlayer) return; //only checks for the locally controlling player

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f)) {
            if (hit.collider.gameObject.tag == Tags.BALL) Debug.Log("Ball in range");
        }
    }

    private void ProcessMouseInput() {
        if(Input.GetMouseButton(0)) {
            //Do stuff
        }
    }
}
