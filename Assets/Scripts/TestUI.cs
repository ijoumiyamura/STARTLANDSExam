using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Cinemachine;
using TMPro;

public class TestUI : MonoBehaviour
{
    [SerializeField] private Button startHostBtn;
    [SerializeField] private Button startClientBtn;
    [SerializeField] private TMP_InputField playerNameInputField;

    public static TestUI Instance { get; private set; }
    private string newText;

    // [SerializeField] private TextMeshPro playerNameText;

    private void Awake() {
        Instance = this;
        startHostBtn.onClick.AddListener(() => {
            GameLobby.Instance.CreateLobby("TestLobby", false);
            // GameMultiplayer.Instance.SetPlayerName(playerNameInputField.text);
            // playerNameText = Player.Instance.nameText;
            // Player.Instance.nameText.text = playerNameInputField.text;
            Hide();
        });

        startClientBtn.onClick.AddListener(() => {
            GameLobby.Instance.QuickJoin();
            // playerNameText = Player.Instance.nameText;
            // playerNameText.text = playerNameInputField.text;
            Hide();
        });
    }

    private void Start() {
        playerNameInputField.text = GameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((newText) => {
            GameMultiplayer.Instance.SetPlayerName(newText);
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    public string GetName(){
        return playerNameInputField.text;
    }
}
