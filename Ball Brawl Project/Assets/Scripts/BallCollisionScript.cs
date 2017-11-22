using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionScript : MonoBehaviour {

    private bool _inRange;
    private GameObject _ball;

    public void OnTriggerEnter(Collider other) {
        if(other.tag == Tags.BALL) {
            if (_ball == null || _ball != other.gameObject) _ball = other.gameObject;

            _inRange = true;
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.tag == Tags.BALL) {
            _inRange = false;
        }
    }

    public bool InRange {
        get { return _inRange; }
    }

    public GameObject Ball {
        get { return _ball; }
    }
}
