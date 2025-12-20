using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;
    [SerializeField]
    public string promptMessage;

    public void BaseInteract()
    {
        if (useEvents)
        {
            InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
            //if (interactionEvent != null)
            //{
                interactionEvent.OnInteract.Invoke();
            //}
        }
        Interact();
    }
    protected virtual void Interact()
    {
        
    }
}
