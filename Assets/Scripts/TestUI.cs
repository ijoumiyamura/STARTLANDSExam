using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Cinemachine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private Button startHostBtn;
    [SerializeField] private Button startClientBtn;

    private void Awake() {
        startHostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Hide();
        });

        startClientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
