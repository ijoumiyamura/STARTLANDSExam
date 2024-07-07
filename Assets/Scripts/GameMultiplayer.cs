using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameMultiplayer : NetworkBehaviour 
{
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static GameMultiplayer Instance { get; private set; }
    
    private string playerName;

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        // playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));
    }

    public string GetPlayerName(){
        return playerName;
    }

    public void SetPlayerName(string playerName){
        this.playerName = playerName;

        // PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    public void StartHost(){
        // NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
        
    }
    public void StartServer(){
        // NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartServer();
        
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse){
        // connectionApprovalResponse.Approved = true;
    }

    public void StartClient(){
        NetworkManager.Singleton.StartClient();
    }
}
