using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamAssignmentScript : NetworkBehaviour {

    [SyncVar]
    private string _assignedTeam;

    public override void OnStartClient() {
        CmdSetTeam(TeamManager.Instance.GetTeam());

        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (_assignedTeam == TeamManager.TEAM_A) {
            renderer.material.color = Color.red;
        } else if (_assignedTeam == TeamManager.TEAM_B) {
            renderer.material.color = Color.blue;
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
