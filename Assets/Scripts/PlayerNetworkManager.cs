using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : NetworkBehaviour
{
    public static PlayerNetworkManager Instance { get; private set; }

    private NetworkList<PlayerData> playerDataList;

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        playerDataList = new NetworkList<PlayerData>();
    }

    // public override void OnNetworkSpawn()
    // {
    //     if (IsServer){
    //         // playerDataList = new NetworkList<PlayerData>();
    //         playerDataList.OnListChanged += OnPlayerDataListChanged;
    //         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    //     }
    // }

    // public override void OnDestroy(){
    //     if (playerDataList != null){
    //         playerDataList.OnListChanged -= OnPlayerDataListChanged;
    //     }

    //     if (NetworkManager.Singleton != null){
    //         NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    //     }
    // }

    // private void OnPlayerDataListChanged(NetworkListEvent<PlayerData> changeEvent){
    //     foreach (var playerData in playerDataList){
    //         UpdatePlayerNameClientRpc(playerData.ClientId, playerData.PlayerName.ToString());
    //     }
    // }

    // private void OnClientConnected(ulong clientId){
    //     foreach (var playerData in playerDataList){
    //         UpdatePlayerNameClientRpc(playerData.ClientId, playerData.PlayerName.ToString(), new ClientRpcParams{
    //             Send = new ClientRpcSendParams{
    //                 TargetClientIds = new List<ulong> { clientId }
    //             }
    //         });
    //     }
        
    // }

    // private void OnClientConnected(ulong clientId){
    //     foreach (var entry in playerNames){
    //         UpdatePlayerNameClientRpc(entry.Key, entry.Value, new ClientRpcParams{
    //             Send = new ClientRpcSendParams{
    //                 TargetClientIds = new List<ulong> { clientId }
    //             }
    //         });
    //         // SetPlayerNameClientRpc(playerName.Key, playerName.Value);
    //     }
    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void RegisterPlayerNameServerRpc(ulong clientId, string playerName){
    //     // playerNames[clientId] = playerName;
    //     // UpdatePlayerNameClientRpc(clientId, playerName);
    //     PlayerData playerData = new PlayerData(clientId, playerName);
    //     playerDataList.Add(playerData);
    //     UpdatePlayerNameClientRpc(clientId, playerName);
    // }

    // [ClientRpc]
    // private void UpdatePlayerNameClientRpc(ulong clientId, string playerName, ClientRpcParams clientRpcParams = default){
    //     Player player = FindPlayer(clientId);
    //     if (player != null){
    //         player.SetPlayerName(playerName);
    //     }
    // }

    // [ClientRpc]
    // private void SetPlayerNameClientRpc(ulong clientId, string playerName){
    //     Player player = FindPlayer(clientId);
    //     if (player != null){
    //         player.SetPlayerName(playerName);
    //     }
    // }

    // private Player FindPlayer(ulong clientId){
    //     foreach (var player in FindObjectsOfType<Player>()){
    //         if (player.OwnerClientId == clientId){
    //             return player;
    //         }
    //     }
    //     return null;
    // }

    // private struct PlayerData : INetworkSerializable, IEquatable<PlayerData>{
    //     public ulong ClientId;
    //     public FixedString32Bytes PlayerName;
    //     public PlayerData(ulong clientId, string playerName){
    //         ClientId = clientId;
    //         PlayerName = playerName;
    //     }

    //     public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{
    //         serializer.SerializeValue(ref ClientId);
    //         serializer.SerializeValue(ref PlayerName);
    //     }

    //     public bool Equals(PlayerData other){
    //         return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName);
    //     }

    //     public override bool Equals(object obj)
    //     {
    //         return obj is PlayerData other && Equals(other);
    //     }

    //     public override int GetHashCode()
    //     {
    //         unchecked{
    //             int hashCode = ClientId.GetHashCode();
    //             hashCode = (hashCode * 397) ^ PlayerName.GetHashCode();
    //             return hashCode;
    //         }
    //     }
    // }
}
