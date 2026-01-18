using UnityEngine;

public class AttackButton : MonoBehaviour
{
    private PlayerAttack playerAttack;

    void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        
        if (playerAttack == null)
        {
            Debug.LogError("[AttackButton] No se encontró PlayerAttack en la escena!");
        }
    }

    public void OnFireButtonPressed()
    {
        if (playerAttack != null)
        {
            playerAttack.Atacar();
        }
        else
        {
            Debug.LogError("[AttackButton] PlayerAttack es null, no se puede atacar!");
        }
    }
}
