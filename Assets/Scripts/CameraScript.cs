using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Unity.VisualScripting;

public class CameraScript : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    public static CameraScript Instance { get; private set;}

    private void Awake() {
        Instance = this;
    }

    public void OnPlayerSpawn(GameObject player){
        cam = GetComponent<CinemachineVirtualCamera>();
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
    }
}
