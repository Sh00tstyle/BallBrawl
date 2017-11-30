using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct PlayerObject {
    public GameObject playerObject; //reference to the object used by the player (needs a network identity)
}

public class PlayerSyncList : SyncListStruct<PlayerObject> {
    //Empty class used to extend from SyncListStruct with the type of a PlayerObject
}
