using UnityEngine;

public class Potion : Interactable
{
    public float healAmount = 25f;

    protected override void Interact()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.RestoreHealth(healAmount);
            Debug.Log("Vida regenerada +" + healAmount);
        }
    }
}
