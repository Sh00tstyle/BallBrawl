using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoalScript : NetworkBehaviour {

    [SerializeField]
    private Material _redGoalMaterial;

    [SerializeField]
    [EventRef]
    private string _goalSound;

    [SerializeField]
    private GameObject _redGoalParticle;

    [SerializeField]
    private GameObject _blueGoalParticle;

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

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Tags.BALL) {
            CmdScorePoint();

            if (_assignedTeam == Teams.TEAM_RED) {
                HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamA, _goalsScored);
                if (isServer) {
                    GameObject redParticle = Instantiate(_redGoalParticle, other.gameObject.transform.position, Quaternion.identity);
                    NetworkServer.Spawn(redParticle);

                    Destroy(redParticle, 2f);
                }
                
            } else {
                AudioManager.GoalScored(Teams.TEAM_BLUE);
                HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamB, _goalsScored);
                if (isServer) {
                    GameObject blueParticle = Instantiate(_blueGoalParticle, other.gameObject.transform.position, Quaternion.identity);
                    NetworkServer.Spawn(blueParticle);

                    Destroy(blueParticle, 2f);
                }
            }
            
            AudioManager.PlayOneShot(_goalSound, other.gameObject);
            GameStateManager.Instance.CmdSetState(GameStates.STATE_SLOWDOWN); //Reset the round whenever a goal is scored
        }
    }

    [Command]
    public void CmdScorePoint() {
        _goalsScored++;
    }

    public void SetTeam(string team) {
        _assignedTeam = team;
    }

    public string AssigenedTeam {
        get { return _assignedTeam; }
    }

    public int goalsScored {
        get { return _goalsScored; }
    }
}
