using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInitScript : NetworkBehaviour {

    [SyncVar]
    private string _assignedTeam;

    public override void OnStartLocalPlayer() {
        AnimationsPlayer _animPlayer = GetComponentInChildren<AnimationsPlayer>();
        SetLayerRecursively(_animPlayer.transform.gameObject, 9);
    }

    public override void OnStartClient() {
        CmdSetTeam(TeamManager.Instance.GetTeam());

        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (_assignedTeam == TeamManager.TEAM_A) {
            renderer.material.color = Color.red;

            GetComponent<PlayerControllerRigidbody>().SetRotationPlayer(180f);
        } else if (_assignedTeam == TeamManager.TEAM_B) {
            renderer.material.color = Color.blue;
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


    [Command]
    public void CmdSetTeam(string team) {
        _assignedTeam = team;
    }

    public string AssignedTeam {
        get { return _assignedTeam; }
    }
}
