using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor;

    public PlayerInput.OnFootActions OnFoot => onFoot;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();

        onFoot.Jump.performed += ctx => motor.Jump();
    }

    void FixedUpdate()
    {
        // tell the playermotor to move using the value from our movement action.
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}