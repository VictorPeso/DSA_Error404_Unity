using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes")]
    public float speed = 5f;

    [Tooltip("Usa esto para corregir la dirección. Prueba 0, 90, -90 o 180.")]
    public float angleOffset = 0f; // <--- ¡LA TUERCA MÁGICA!

    private CharacterController controller;
    private Vector2 moveInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. LEER EL JOYSTICK
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
        }
        else
        {
            // Fallback Teclado
            moveInput = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) moveInput.y = 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y = -1;
            if (Keyboard.current.aKey.isPressed) moveInput.x = -1;
            if (Keyboard.current.dKey.isPressed) moveInput.x = 1;
        }

        // Si no te mueves, no calculamos nada
        if (moveInput.magnitude < 0.1f) return;

        // 2. CORREGIR LA DIRECCIÓN MANUALMENTE
        // Convertimos el Joystick (X,Y) en Dirección 3D (X,0,Z)
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y);

        // Aplicamos la rotación extra que tú configures en el Inspector
        // Quaternion.Euler(0, angleOffset, 0) gira el vector como una brújula
        Vector3 finalDir = Quaternion.Euler(0, angleOffset, 0) * inputDir;

        // 3. MOVER
        controller.Move(finalDir * speed * Time.deltaTime);

        // Girar el personaje hacia donde anda
        transform.forward = finalDir;
    }
}