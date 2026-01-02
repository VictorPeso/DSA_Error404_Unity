using UnityEngine;

public class Potion : Interactable
{
    public float healAmount = 25f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

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
