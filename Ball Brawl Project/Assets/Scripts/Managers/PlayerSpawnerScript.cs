using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnerScript : MonoBehaviour {

    [SerializeField]
    private Transform _teamASpawn;

    [SerializeField]
    private Transform _teamBSpawn;

    public Transform TeamASpawn {
        get { return _teamASpawn; }
    }

    public Transform TeamBSpawn {
        get { return _teamBSpawn; }
    }
}
