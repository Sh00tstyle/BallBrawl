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

        if (playerId % 2 == 1) playerTeamScript.CmdSetTeam(Teams.TEAM_A);
        else playerTeamScript.CmdSetTeam(Teams.TEAM_B);

        _playerList.Add(playerObj);

        //Initialize the round, as soon as two players have been registered
        if (PlayerCount >= 2) GameStateManager.Instance.CmdSetState(GameStates.STATE_READYROUND);
    }

    [Command]
    public void CmdUnregisterPlayer(int index) {
        _playerList.RemoveAt(index);
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
