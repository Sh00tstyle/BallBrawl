using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTeamScript : NetworkBehaviour {

    [SyncVar]
    private string _assignedTeam;

    [Command]
    public void CmdSetTeam(string team) {
        _assignedTeam = team;
    }

    public void Start() {
        ApplyTeamColor();
    }

    public void ApplyTeamColor() {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (_assignedTeam == Teams.TEAM_A) {
            renderer.material.color = Color.red;

            GetComponent<PlayerControllerRigidbody>().SetRotationPlayer(180f);
        } else if (_assignedTeam == Teams.TEAM_B) {
            renderer.material.color = Color.blue;
        }
    }

    public string AssignedTeam {
        get { return _assignedTeam; }
    }
}
