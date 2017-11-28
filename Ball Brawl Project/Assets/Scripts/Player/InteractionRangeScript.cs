using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRangeScript : MonoBehaviour {

    private bool _ballInRange;
    private bool _playerInRange;

    private GameObject _playerObject;

    public void OnTriggerEnter(Collider other) {
        if(other.tag == Tags.BALL) {
            _ballInRange = true;
        }
        
        if(other.tag == Tags.PLAYER) {
            _playerInRange = true;
            _playerObject = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.tag == Tags.BALL) {
            _ballInRange = false;
        }

        if (other.tag == Tags.PLAYER) {
            _playerInRange = false;
            _playerObject = null;
        }
    }

    public bool BallInRange {
        get { return _ballInRange; }
    }

    public bool PlayerInRange {
        get { return _playerInRange; }
    }

    public GameObject PlayerObject {
        get { return _playerObject; }
    }
}
