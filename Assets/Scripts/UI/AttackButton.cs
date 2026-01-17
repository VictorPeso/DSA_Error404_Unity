using UnityEngine;

/// <summary>
/// Script para botón UI de ataque
/// Llama al método Atacar() del Player cuando se presiona el botón
/// </summary>
public class AttackButton : MonoBehaviour
{
    private PlayerAttack playerAttack;

    void Start()
    {
        // Buscar el PlayerAttack en la escena
        playerAttack = FindObjectOfType<PlayerAttack>();
        
        if (playerAttack == null)
        {
            Debug.LogError("[AttackButton] No se encontró PlayerAttack en la escena!");
        }
        else
        {
            Debug.Log("[AttackButton] PlayerAttack encontrado correctamente");
        }
    }

    /// <summary>
    /// Método público que se llama desde el evento OnClick del botón UI
    /// </summary>
    public void OnFireButtonPressed()
    {
        if (playerAttack != null)
        {
            Debug.Log("[AttackButton] Botón de disparo presionado!");
            playerAttack.Atacar();
        }
        else
        {
            Debug.LogError("[AttackButton] PlayerAttack es null, no se puede atacar!");
        }
    }
}
