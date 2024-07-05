using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private GameInput gameInput;
    private bool isWalking = false;
    private void Update()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
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
