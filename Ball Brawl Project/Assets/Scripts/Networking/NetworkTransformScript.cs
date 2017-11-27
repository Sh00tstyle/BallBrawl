using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel=1, sendInterval = 0.05f)]
public class NetworkTransformScript : NetworkTransform {
}
