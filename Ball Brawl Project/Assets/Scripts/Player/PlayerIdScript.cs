using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerIdScript : NetworkBehaviour {

    [SyncVar]
    private int _id;

    [Command]
    public void CmdAssignId(int newId) {
        _id = newId;
    }

    public int ID {
        get { return _id; }
    }
}
