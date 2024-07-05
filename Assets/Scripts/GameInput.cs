using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions; 

    public static GameInput Instance { get; private set; }

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();    

        Instance = this;
    }

    public Vector2 GetMovementVectorNormalized(){
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // if (Input.GetKey(KeyCode.LeftShift)){
        //     isRunning = true;
        // }

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public Vector2 GetAimVectorNormalized(){
        Vector2 inputVector = playerInputActions.Player.Aim.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public bool Shoot(){
        return Input.GetKey(KeyCode.Space);
    }
}
