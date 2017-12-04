using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using FMODUnity;

public class GoalCollisionScript : NetworkBehaviour {

    [SerializeField]
    [EventRef]
    private string _goalSound;

    [SerializeField]
    private GameObject _redGoalParticle;

    [SerializeField]
    private GameObject _blueGoalParticle;

    private GoalScript _goalScript;

    public void Awake() {
        _goalScript = GetComponentInParent<GoalScript>();
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Tags.BALL) {
            _goalScript.CmdScorePoint();

            if (_goalScript.AssignedTeam == Teams.TEAM_RED) {
                HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamBlue, _goalScript.GoalsScored);
                if (_goalScript.isServer) {
                    GameObject redParticle = Instantiate(_redGoalParticle, other.gameObject.transform.position, Quaternion.identity);
                    NetworkServer.Spawn(redParticle);

                    Destroy(redParticle, 2f);
                }

            } else {
                AudioManager.GoalScored(Teams.TEAM_BLUE);
                HudOverlayManager.Instance.UpdateGoalCount(HudOverlayManager.HUDText.CounterTeamRed, _goalScript.GoalsScored);
                if (_goalScript.isServer) {
                    GameObject blueParticle = Instantiate(_blueGoalParticle, other.gameObject.transform.position, Quaternion.identity);
                    NetworkServer.Spawn(blueParticle);

                    Destroy(blueParticle, 2f);
                }
            }

            AudioManager.PlayOneShot(_goalSound, other.gameObject);
            GameStateManager.Instance.CmdSetState(GameStates.STATE_SLOWDOWN); //Reset the round whenever a goal is scored
        }
    }

}
