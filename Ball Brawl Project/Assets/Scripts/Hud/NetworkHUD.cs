using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("Network/NetworkCustomHUD")]
[RequireComponent(typeof(NetworkManager))]
public class NetworkHUD : MonoBehaviour {

    private NetworkManager _manager;

    public Texture Host;
    public Texture HostHover;
    public Texture Join;
    public Texture JoinHover;
    public Texture Server;
    public Texture ServerHover;

    public GUISkin MenuSkin;
    public GameObject IngameUI;

    public Camera CinematicCamera;
    public GameObject CameraOrigin;
    public float CameraHeightOffset;
    public float CameraDistanceOffset;
    public float CameraRotationSpeed;

    private void Awake() {
        _manager = GetComponent<NetworkManager>();
        CinematicCamera.transform.position = new Vector3(CameraOrigin.transform.position.x + CameraDistanceOffset, CameraOrigin.transform.position.y + CameraHeightOffset, 0);
    }

    void Update() {
        //Handle Key Input
        if (!NetworkClient.active && !NetworkServer.active && _manager.matchMaker == null) {
            if (Input.GetKeyDown(KeyCode.S)) {
                _manager.StartServer();
                OnSelect();
            }
            if (Input.GetKeyDown(KeyCode.H)) {
                _manager.StartHost();
                OnSelect();
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                _manager.StartClient();
                OnSelect();
            }
        }
        if (NetworkServer.active && NetworkClient.active) {
            if (Input.GetKeyDown(KeyCode.X)) {
                _manager.StopHost();
                CinematicCamera.enabled = true;
                IngameUI.SetActive(false);
            }
        }

        CinematicCamera.transform.RotateAround(CameraOrigin.transform.position, transform.up, Time.deltaTime * CameraRotationSpeed);
        CinematicCamera.transform.LookAt(CameraOrigin.transform.position);
    }

    void OnGUI() {

        int spacing = 50;
        int startScreenX = (Screen.width / 2) - 100;
        int startScreenY = (Screen.height / 2) - ((spacing * 3) / 2);// - (crosshairImage.height / 2);
        int onlineX = 10;
        int onlineY = 40;

        //If nothing is active = Start Screen
        if (!NetworkClient.active && !NetworkServer.active && _manager.matchMaker == null) {

            //Host Button
            Rect HostRect = new Rect(startScreenX, startScreenY, 200, 40);                       
            Texture HostTexture = Host;
            if (HostRect.Contains(Event.current.mousePosition)) HostTexture = HostHover;
            else HostTexture = Host;

            if (GUI.Button(HostRect, HostTexture, MenuSkin.button)) {
                _manager.StartHost();
                OnSelect();
            }
            startScreenY += spacing;

            //Join Button
            Rect JoinRect = new Rect(startScreenX, startScreenY, 105, 40);
            Texture JoinTexture;
            if (JoinRect.Contains(Event.current.mousePosition)) JoinTexture = JoinHover;
            else JoinTexture = Join;

            if (GUI.Button(JoinRect, JoinTexture, MenuSkin.button)) {
                _manager.StartClient();
                OnSelect();
            }
            _manager.networkAddress = GUI.TextField(new Rect(startScreenX + 105, startScreenY + 10, 95, 20), _manager.networkAddress);
            startScreenY += spacing;

            //Server Button
            Rect ServerRect = new Rect(startScreenX, startScreenY, 200, 40);
            Texture ServerTexture;
            if (ServerRect.Contains(Event.current.mousePosition)) ServerTexture = ServerHover;
            else ServerTexture = Server;

            if (GUI.Button(ServerRect, ServerTexture, MenuSkin.button)) {
                _manager.StartServer();
                OnSelect();

            }
            startScreenY += spacing;
        }

        //If Client is active, display the Client IP Address and port, if the server is active display the port its running on
        else {
            if (NetworkServer.active) {
                GUI.Label(new Rect(onlineX, onlineY, 300, 20), "Server: port=" + _manager.networkPort);
                onlineY += spacing;
            }
            if (NetworkClient.active) {
                GUI.Label(new Rect(onlineX, onlineY, 300, 20), "Client: address=" + _manager.networkAddress + " port=" + _manager.networkPort);
                onlineY += spacing;
            }
        }

        //If you are online, but wanna disconnect
        if (NetworkServer.active || NetworkClient.active) {
            if (GUI.Button(new Rect(onlineX, onlineY, 200, 20), "Stop (X)")) {
                _manager.StopHost();
            }
            onlineY += spacing;
        }
    }

    private void OnSelect() {
        CinematicCamera.enabled = false;
        IngameUI.SetActive(true);
    } 
}
