using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player: NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Weapon weaponPrefab;

    public static Player Instance { get; private set; }
    private bool isWalking = false;

    private void Awake() {
        Instance = this;
    }
    private void Update()
    {
        if (!IsOwner) return;
        HandleMovement();
        // HandleMovementServerAuth();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsLocalPlayer){
            CameraScript.Instance.OnPlayerSpawn(gameObject);
            Weapon weapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
            weapon.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void HandleMovementServerAuth(){
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        HandleMovementServerRpc(inputVector);
    }

    [ServerRpc]
    private void HandleMovementServerRpc(Vector2 inputVector){
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        

        isWalking = moveDirection != Vector3.zero;

        float rotateSpeed = 13f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        
    }

    private void HandleMovement(){
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        

        isWalking = moveDirection != Vector3.zero;

        float rotateSpeed = 13f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        
    } 

    public bool IsWalking(){
        return isWalking;
    }
}
