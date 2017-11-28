using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Makes the NetworkTransform use another channel at a different sendInterval
[NetworkSettings(channel=1, sendInterval = 0.05f)]
public class NetworkTransformScript : NetworkTransform {
}
