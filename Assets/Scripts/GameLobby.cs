using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Animations;

public class GameLobby : MonoBehaviour
{
    public static GameLobby Instance { get; private set; }

    private Lobby joinedLobby;
    private float hearbeatTimer;

    private void Awake(){
        Instance = this;

        // DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private void Update() {
        HandleHeartBeat();
    }

    private void HandleHeartBeat(){
        if (IsLobbyHost()){
            hearbeatTimer -= Time.deltaTime;
            if (hearbeatTimer <= 0f){
                float hearbeatTimerMax = 15f;
                hearbeatTimer = hearbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
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

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate){
        try{
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 10, new CreateLobbyOptions{
                IsPrivate = isPrivate,
            });

            GameMultiplayer.Instance.StartHost();
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void QuickJoin(){
        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            GameMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }
}
