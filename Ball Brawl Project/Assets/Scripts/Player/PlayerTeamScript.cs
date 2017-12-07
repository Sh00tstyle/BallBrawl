using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTeamScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _playerMeshHolder;

    [SerializeField]
    private GameObject _teamMeshHolder;

    [SerializeField]
    private GameObject _rightHand;

    [SerializeField]
    private Material _ball2BlueMat;

    [SerializeField]
    private Material _ball3BlueMat;

    [SerializeField]
    private Material _redTeamBodyMat;

    [SerializeField]
    private Material _redTeamHoodMat;

    [SerializeField]
    private Material _redHandMat;

    [SerializeField]
    private Material _redBraceletMat;

    [SyncVar]
    private string _assignedTeam;

    [Command]
    public void CmdSetTeam(string team) {
        _assignedTeam = team;
    }

    public void Start() {
        ApplyTeamColor();
    }

    public override void OnStartLocalPlayer() {
        SetLayerRecursively(_playerMeshHolder, 9);
    }

    public void ApplyTeamColor() {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (_assignedTeam == Teams.TEAM_RED) {

            Transform[] bodyChildren = _teamMeshHolder.GetComponentsInChildren<Transform>();
            foreach(Transform t in bodyChildren) {
                if (t.gameObject.GetComponent<SkinnedMeshRenderer>()) {
                    if(t.gameObject.tag == Tags.BODY) t.gameObject.GetComponent<SkinnedMeshRenderer>().material = _redTeamBodyMat;
                    if(t.gameObject.tag == Tags.HOOD) t.gameObject.GetComponent<SkinnedMeshRenderer>().material = _redTeamHoodMat;
                }
            }

            Transform[] handChildren = _rightHand.GetComponentsInChildren<Transform>();
            foreach (Transform t in handChildren) {
                if (t.gameObject.GetComponent<SkinnedMeshRenderer>()) {
                    if (t.gameObject.tag == Tags.HAND) t.gameObject.GetComponent<SkinnedMeshRenderer>().material = _redHandMat;
                    if (t.gameObject.tag == Tags.BRACELET) t.gameObject.GetComponent<SkinnedMeshRenderer>().material = _redBraceletMat;
                }
            }

            GetComponent<PlayerControllerRigidbody>().SetRotationPlayer(180f);
        } else if (_assignedTeam == Teams.TEAM_BLUE) {
            renderer.material.color = Color.blue;

            PlayerInteractionScript playerInteraction = GetComponent<PlayerInteractionScript>();

            MeshRenderer[] localBallRenderers = playerInteraction.LocalBall.GetComponentsInChildren<MeshRenderer>();
            localBallRenderers[0].material = _ball2BlueMat;
            localBallRenderers[1].material = _ball3BlueMat;

            MeshRenderer[] visualBallRenderers = playerInteraction.VisualBall.GetComponentsInChildren<MeshRenderer>();
            visualBallRenderers[0].material = _ball2BlueMat;
            visualBallRenderers[1].material = _ball3BlueMat;
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer) {
        if (null == obj) {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform) {
            if (null == child) {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public string AssignedTeam {
        get { return _assignedTeam; }
    }
}
