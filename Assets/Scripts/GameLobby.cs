using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Multiplay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Animations;

public class GameLobby : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public static GameLobby Instance { get; private set; }

    private Lobby joinedLobby;
    private float hearbeatTimer;
    private string ipv4Address;
    private ushort port;

    #if DEDICATED_SERVER
        private float autoAllocateTimer = 9999999f;
        private bool alreadyAutoAllocated;
        private static IServerQueryHandler serverQueryHandler;
    #endif

    private void Awake(){
        Instance = this;

        // DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private void Update() {
        HandleHeartBeat();

#if DEDICATED_SERVER
        autoAllocateTimer -= Time.deltaTime;
        if (autoAllocateTimer <= 0f){
            autoAllocateTimer = 999f;
            MultiplayEventCallbacks_Allocate(null);
        }

        if (serverQueryHandler != null){
            if (NetworkManager.Singleton.IsServer){
                serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
            }
            serverQueryHandler.UpdateServerCheck();
        }
#endif        
    }

    private void HandleHeartBeat(){
        if (IsLobbyHost()){
            hearbeatTimer -= Time.deltaTime;
            if (hearbeatTimer <= 0f){
                float hearbeatTimerMax = 15f;
                hearbeatTimer = hearbeatTimerMax;

                if (joinedLobby != null){
                    LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }
    }

    private bool IsLobbyHost(){
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void InitializeUnityAuthentication(){
        if (UnityServices.State != ServicesInitializationState.Initialized){
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

#if !DEDICATED_SERVER
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
#endif

#if DEDICATED_SERVER
            Debug.Log("DEDICATED_SERVER LOBBY");

            MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
            multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
            multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
            multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
            multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
            IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

            serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(10, "MyServerName", "STARTLANDSExam", "V1", "map");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != ""){
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));

            }
#endif      
        } else {
#if DEDICATED_SERVER
            Debug.Log("DEDICATED_SERVER LOBBY ALREADY INIT");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != ""){
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
            }
#endif
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate){
        try{
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 10, new CreateLobbyOptions{
                IsPrivate = isPrivate,
            });

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            
            GameMultiplayer.Instance.StartHost();
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void QuickJoin(){
        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);
            
            GameMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async Task<Allocation> AllocateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(9);

            return allocation;
        } catch (RelayServiceException e){
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation){
        try{
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        } catch (RelayServiceException e){
            Debug.Log(e);

            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            return joinAllocation;
        } catch (RelayServiceException e){
            Debug.Log(e);

            return default;
        }
    }

#if DEDICATED_SERVER
    private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj){
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
        Debug.Log(obj);
    }

    private void MultiplayEventCallbacks_Error(MultiplayError obj){
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
        Debug.Log(obj.Reason);
    }

    private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj){
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
    }
    private async void MultiplayEventCallbacks_Allocate(MultiplayAllocation obj){
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Allocate");

        if (alreadyAutoAllocated){
            Debug.Log("Alreadu auto allocated!");
            return;
        }

        alreadyAutoAllocated = true;

        var serverConfig = MultiplayService.Instance.ServerConfig;
        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        Debug.Log($"Port[{serverConfig.Port}]");
        Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

        ipv4Address = "0.0.0.0";
        port = serverConfig.Port;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");
        
        GameMultiplayer.Instance.StartServer();

        await MultiplayService.Instance.ReadyServerForPlayersAsync();
        Camera.main.enabled = false; 

    }
#endif
}
