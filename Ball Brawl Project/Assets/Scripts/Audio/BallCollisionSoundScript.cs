using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Networking;

public class BallCollisionSoundScript : NetworkBehaviour {

    [SerializeField]
    [EventRef]
    public string _sound;

    public void Start() {
        AudioManager.PlayOneShot(_sound, gameObject);
    }
}
