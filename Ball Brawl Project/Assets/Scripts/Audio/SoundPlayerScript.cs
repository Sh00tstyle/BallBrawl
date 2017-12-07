using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Networking;

public class SoundPlayerScript : NetworkBehaviour {

    [SerializeField]
    [EventRef]
    public string _sound;

    public bool isCrowd;
    public string team;

    public void Start() {
        if (isCrowd) AudioManager.CrowdMiss(team);
        else AudioManager.PlayOneShot(_sound, gameObject);
    }
}
