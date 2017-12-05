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
        //Handle Key Input
        if (!NetworkClient.active && !NetworkServer.active) {
            if (Input.GetKeyDown(KeyCode.S)) {
                OnSelect();
                _manager.StartServer();
            }
            if (Input.GetKeyDown(KeyCode.H)) {
                OnSelect();
                _manager.StartHost();
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                OnSelect();
                _manager.StartClient();
            }
        }
        if (NetworkServer.active && NetworkClient.active) {
            if (Input.GetKeyDown(KeyCode.X)) {
                OnEnterMenu();
                _manager.StopHost();
            }
        }

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
        _manager.StartClient();
        //Read out the user Input
        string IP = UserInput.GetComponent<Text>().text;
        //If the user Input is empty, take the default predetermined IP Address aka. localhost
        if (IP == null) IP = DefaultText.GetComponent<Text>().text;
        _manager.networkAddress = IP;
        AudioManager.setParameter(_menuMusic, gameObject, "Volume", 0f);
    }

    public void OnClickQuit() {
        OnSelect();
        Application.Quit();
    }
}
