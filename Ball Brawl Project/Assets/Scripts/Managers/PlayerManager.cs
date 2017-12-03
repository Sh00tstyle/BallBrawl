using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

    private static PlayerManager _instance;

    //Automatically sync the list over the network
    private PlayerSyncList _playerList;

    public void Awake() {
        if(_instance == null) {
            _instance = this;

            _playerList = new PlayerSyncList(); //only create this once (?)
        }
    }

    [Command]
    public void CmdRegisterPlayer(int playerId, GameObject playerObject) {
        PlayerObject playerObj = new PlayerObject();
        playerObj.playerObject = playerObject;

        PlayerIdScript playerIdScript = playerObject.GetComponent<PlayerIdScript>();
        playerIdScript.CmdAssignId(playerId);

        PlayerTeamScript playerTeamScript = playerObject.GetComponent<PlayerTeamScript>();

        if (playerId % 2 == 1) playerTeamScript.CmdSetTeam(Teams.TEAM_RED);
        else playerTeamScript.CmdSetTeam(Teams.TEAM_BLUE);

        _playerList.Add(playerObj);

        //Initialize the round, as soon as two players have been registered
        if (PlayerCount >= 2) GameStateManager.Instance.CmdResetMatch();
    }

    [Command]
    public void CmdUnregisterPlayer(int index) {
        if (_playerList.Count < index + 1) return; //Prevent removing non existen objects

        _playerList.RemoveAt(index);

        if (PlayerCount < 2) GameStateManager.Instance.CmdSetState(GameStates.STATE_IDLE); //The other client waits and then the match will be restarted
    }

    public PlayerObject GetPlayerAt(int index) {
        return _playerList[index];
    }

    public int PlayerCount {
        get { return _playerList.Count; }
    }

    public static PlayerManager Instance {
        get { return _instance; }
    }
}
