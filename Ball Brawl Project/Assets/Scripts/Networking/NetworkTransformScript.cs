using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Makes the NetworkTransform use another channel at a different sendInterval
[NetworkSettings(channel = 2)]
public class NetworkTransformScript : NetworkTransform {
}
