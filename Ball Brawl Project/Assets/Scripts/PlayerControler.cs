using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {

    private Rigidbody _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer() {
        Camera camera = GetComponentInChildren<Camera>();
        camera.enabled = true;
    }

    public void Update() {
        if (!isLocalPlayer) return;

        ProcessControls();
    }

    private void ProcessControls() {
        if (Input.GetKey(KeyCode.W)) {
            Debug.Log("w");
            _rigidbody.AddRelativeForce(new Vector3(0, 0, 5));
        } else if (Input.GetKey(KeyCode.S)) {
            Debug.Log("s");
            _rigidbody.AddRelativeForce(new Vector3(0, 0, -5));
        }

        if(Input.GetKey(KeyCode.A)) {
            Debug.Log("a");
            _rigidbody.AddRelativeForce(new Vector3(-5, 0, 0));
        } else if(Input.GetKey(KeyCode.D)) {
            Debug.Log("d");
            _rigidbody.AddRelativeForce(new Vector3(5, 0, 0));
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            _rigidbody.AddRelativeForce(new Vector3(0, 500, 0));
            Debug.Log("jump");
        }
    }
}
