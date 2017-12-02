using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTeamScript : NetworkBehaviour {

    [SerializeField]
    private GameObject _playerMeshHolder;

    [SerializeField]
    private Material _ball2BlueMat;

    [SerializeField]
    private Material _ball3BlueMat;

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
            renderer.material.color = Color.red; //will have to be changed for the real model

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
