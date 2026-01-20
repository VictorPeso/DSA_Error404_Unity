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
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Vector3 rayDirection = transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * distance, Color.red);

        RaycastHit hitInfo;
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
