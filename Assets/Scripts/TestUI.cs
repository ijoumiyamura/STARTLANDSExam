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

    // [SerializeField] private TextMeshPro playerNameText;

    private void Awake() {
        startHostBtn.onClick.AddListener(() => {
            GameLobby.Instance.CreateLobby("TestLobby", false);
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
        playerNameInputField.onValueChanged.AddListener((string newText) => {
            GameMultiplayer.Instance.SetPlayerName(newText);
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
