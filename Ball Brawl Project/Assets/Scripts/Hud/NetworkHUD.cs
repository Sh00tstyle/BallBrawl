using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[AddComponentMenu("Network/NetworkCustomHUD")]
public class NetworkHUD : MonoBehaviour {

    [EventRef]
    public string _menuMusic;

    public GameObject ManagerObject;
    private NetworkManagerScript _manager;

    public GameObject MenuUI;
    public GameObject IngameUI;

    public GameObject DefaultText;
    public GameObject UserInput;

    public Camera CinematicCamera;
    public GameObject CameraOrigin;
    public float CameraHeightOffset;
    public float CameraDistanceOffset;
    public float CameraRotationSpeed;

    private void Start() {
        _manager = ManagerObject.GetComponent<NetworkManagerScript>();
        IngameUI.SetActive(false);
        CinematicCamera.transform.position = new Vector3(CameraOrigin.transform.position.x + CameraDistanceOffset, CameraOrigin.transform.position.y + CameraHeightOffset, 0);
        AudioManager.PlayEvent(_menuMusic, gameObject, false, false);
    }

    void Update() {
        //Spin the camera around the origin
        CinematicCamera.transform.RotateAround(CameraOrigin.transform.position, transform.up, Time.deltaTime * CameraRotationSpeed);
        CinematicCamera.transform.LookAt(CameraOrigin.transform.position);
    }

    //Enable Menu, disable Ingame Hud
    private void OnEnterMenu() {
        MenuUI.SetActive(true);
        CinematicCamera.enabled = true;
        IngameUI.SetActive(false);
    }

    //Disable Menu, enable Ingame Hud
    private void OnSelect() {
        MenuUI.SetActive(false);
        CinematicCamera.enabled = false;
        IngameUI.SetActive(true);
    }

    public void OnClickHost() {
        OnSelect();
        _manager.StartHost();
        AudioManager.setParameter(_menuMusic, gameObject, "Volume", 0f);
    }

    public void OnClickJoin() {
        OnSelect();
        
        //Read out the user Input
        string IP = UserInput.GetComponent<Text>().text;
        //If the user Input is empty, take the default predetermined IP Address aka. localhost
        if (IP == "") IP = DefaultText.GetComponent<Text>().text;

        //This is how you actually do it
        _manager.networkAddress = IP;
        _manager.networkPort = 7777; //default port we are using

        _manager.StartClient();

        AudioManager.setParameter(_menuMusic, gameObject, "Volume", 0f);
    }

    public void OnDisconnect() {
        OnEnterMenu();

        _manager.StopHost();
    }

    public void OnClickQuit() {
        OnSelect();
        Application.Quit();
    }
}
