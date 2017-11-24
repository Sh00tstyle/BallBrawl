using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings]
public class NetworkTransformScript : NetworkTransform {

    //Forcing the NetworkTransform to use channel 1 with state transform
    public override int GetNetworkChannel() {
        return 1;
    }
}
