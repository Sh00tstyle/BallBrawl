using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalScript : NetworkBehaviour {
    [SerializeField]
    [EventRef]
    private string _goalImpact;

    [SerializeField]
    private GameObject _goalMissPrefab;

    [SerializeField]
    private GameObject _goalImpactPrefab;

    [SerializeField]
    private Material _redGoalMaterial;

    [SyncVar]
    private int _goalsScored;

    [SyncVar]
    private string _assignedTeam;

    public override void OnStartClient() {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (_assignedTeam == Teams.TEAM_RED) {
            renderer.material.color = Color.red;

            ParticleSystem particleSys = GetComponentInChildren<ParticleSystem>();
            Renderer particleRenderer = particleSys.GetComponent<Renderer>();
            particleRenderer.material = _redGoalMaterial;
        } else if (_assignedTeam == Teams.TEAM_BLUE) {
            renderer.material.color = Color.blue;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == Tags.BALL && isServer)
        {
            GameObject goalMiss = Instantiate(_goalMissPrefab, transform.position, Quaternion.LookRotation(collision.contacts[0].normal, transform.up));
            SoundPlayerScript soundPlayer = goalMiss.GetComponent<SoundPlayerScript>();
            soundPlayer.team = _assignedTeam;

            NetworkServer.Spawn(goalMiss);
            Destroy(goalMiss, 0.5f);

            GameObject goalImpact = Instantiate(_goalImpactPrefab, transform.position, Quaternion.LookRotation(collision.contacts[0].normal, transform.up));
            NetworkServer.Spawn(goalImpact);
            Destroy(goalImpact, 0.5f);
        }
    }

    public void Start() {
        if (_assignedTeam == Teams.TEAM_RED)HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamRed, _goalsScored);
        else if (_assignedTeam == Teams.TEAM_BLUE) HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamBlue, _goalsScored);
    }

    [Command]
    public void CmdScorePoint() {
        _goalsScored++;
    }

    [Command]
    public void CmdReset() {
        _goalsScored = 0;
    }

    public void SetTeam(string team) {
        _assignedTeam = team;
    }

    public string AssignedTeam {
        get { return _assignedTeam; }
    }

    public int GoalsScored {
        get { return _goalsScored; }
    }
}
