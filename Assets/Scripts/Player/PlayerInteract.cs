using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;

    private InputManager inputManager;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        // LÓGICA TOP-DOWN:
        // El rayo sale de la posición del jugador (+ 1 metro de altura para que no salga de los pies)
        // Y va hacia donde mira el jugador (transform.forward)
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Vector3 rayDirection = transform.forward;

        // Dibujamos el rayo en rojo en la escena para que veas si funciona
        Debug.DrawRay(rayOrigin, rayDirection * distance, Color.red);

        RaycastHit hitInfo;
        // Lanzamos el rayo desde el cuerpo, no desde la cámara
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();

                if (inputManager.OnFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
