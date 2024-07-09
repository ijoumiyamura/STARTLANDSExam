using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameMultiplayer : NetworkBehaviour 
{
    
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static GameMultiplayer Instance { get; private set; }

    public event EventHandler OnPlayerDataNetworkListChanged;
    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "Player " + UnityEngine.Random.Range(100, 1000));

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent){
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public string GetPlayerName(){
        return playerName;
    }

    public void SetPlayerName(string playerName){
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    public void StartHost(){
        // NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId){
        foreach (var entry in playerDataNetworkList){
            if (entry.clientId == clientId){
                return entry;
            }         
        }
        
        // return playerDataNetworkList[(int)clientId];
        return default;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId){
        playerDataNetworkList.Add(new PlayerData{
            clientId = clientId,
            playerName = playerName,
        });
        SetPlayerNameServerRpc(GetPlayerName());
    }

    public void StartServer(){
        // NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartServer();
        
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse){
        // connectionApprovalResponse.Approved = true;
    }

    public void StartClient(){
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId){
        SetPlayerNameServerRpc(GetPlayerName());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default){
        // ulong playerDataClientId = serverRpcParams.Receive.SenderClientId;

        PlayerData playerData = GetPlayerDataFromClientId(serverRpcParams.Receive.SenderClientId);

        playerData.playerName = playerName;
        playerDataNetworkList[(int)playerData.clientId] = playerData;

        // foreach (var entry in playerDataNetworkList){
        //     if (entry.clientId == playerDataClientId){
        //         PlayerData playerData = entry;
        //         playerData.playerName = playerName;
        //     }
        // }
    }
}
