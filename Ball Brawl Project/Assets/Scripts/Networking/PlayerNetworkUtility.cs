using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetworkUtility : MonoBehaviour {

    private Text _text;

	void Start () {
        _text = GetComponent<Text>();

        //Determine the IP Address of your ethernet port
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()) {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                //print(ni.Name);
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses) {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                        _text.text = ip.Address.ToString();
                    }
                }
            }
        }
    }
}
