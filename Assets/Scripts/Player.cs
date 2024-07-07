using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using TMPro;

public class Player: NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private TextMeshPro playerNameText;
    public static Player Instance { get; private set; }
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isPunching = false;
    private void Awake() {
        Instance = this;
    }
    private void Update()
    {
        if (!IsOwner) return;
        HandleMovement();
        isPunching = GameInput.Instance.Punch();
        if (isPunching){
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f)){
                Damagable damagable = hit.collider.GetComponent<Damagable>();
                if (damagable != null){
                    damagable.TakeDamage();
                    isPunching = false;
                }
            }
            
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsLocalPlayer){
            CameraScript.Instance.OnPlayerSpawn(gameObject);
        }
    }
    public NetworkObject GetNetworkObject(){
        return NetworkObject;
    }

    private void HandleMovement(){
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        isRunning = GameInput.Instance.Run();
        
        if (isRunning){
            Move(runSpeed, moveDirection);  
        }
        else{
            Move(moveSpeed, moveDirection);    
        }
        isWalking = moveDirection != Vector3.zero;

        float rotateSpeed = 13f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    } 

    private void Move(float speed, Vector3 moveDirection){
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 1f, 0.25f, moveDirection, speed * Time.deltaTime);
        if (!canMove){
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0);
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 1f, 0.25f, moveDirX, speed * Time.deltaTime);
            if (canMove){
                moveDirection = moveDirX;
            }
            else{
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 1f, 0.25f, moveDirZ, speed * Time.deltaTime);

                if (canMove){
                    moveDirection = moveDirZ;
                }
            }
        }

        if (canMove){
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    public bool IsWalking(){
        return isWalking;
    }

    public bool IsRunning(){
        return isRunning;
    }
}
