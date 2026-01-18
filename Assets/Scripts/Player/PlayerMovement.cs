using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes")]
    public float speed = 5f;

    [Tooltip("Usa esto para corregir la dirección.")]
    public float angleOffset = 0f;

    private CharacterController controller;
    private Vector2 moveInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
        }
        else
        {
            moveInput = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) moveInput.y = 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y = -1;
            if (Keyboard.current.aKey.isPressed) moveInput.x = -1;
            if (Keyboard.current.dKey.isPressed) moveInput.x = 1;
        }

        if (moveInput.magnitude < 0.1f) return;

        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 finalDir = Quaternion.Euler(0, angleOffset, 0) * inputDir;

        controller.Move(finalDir * speed * Time.deltaTime);

        transform.forward = finalDir;
    }
}