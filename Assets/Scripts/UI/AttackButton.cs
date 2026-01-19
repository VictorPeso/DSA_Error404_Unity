using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class AttackButton : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private Button button;
    
    void Awake()
    {
        button = GetComponent<Button>();
    }
    
    void Start()
    {
        FindPlayerAttack();
        
        if (button != null)
        {
            button.onClick.AddListener(OnAttackClicked);
        }
    }
    
    void FindPlayerAttack()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        
        if (playerAttack == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerAttack = player.GetComponent<PlayerAttack>();
            }
        }
        
        if (playerAttack == null)
        {
            Debug.LogError("[AttackButton] PlayerAttack NO encontrado!");
            Debug.Log("[AttackButton] Buscando GameObjects en escena:");
            
            foreach (var obj in FindObjectsOfType<GameObject>())
            {
                if (obj.activeInHierarchy && obj.GetComponent<PlayerAttack>() != null)
                {
                    Debug.Log($"[AttackButton] Encontrado PlayerAttack en: {obj.name}");
                    playerAttack = obj.GetComponent<PlayerAttack>();
                }
            }
        }
    }
    
    public void OnFireButtonPressed()
    {
        OnAttackClicked();
    }
    
    void OnAttackClicked()
    {
        Debug.Log("[AttackButton] Botón presionado");
        
        if (playerAttack == null)
        {
            Debug.LogError("[AttackButton] playerAttack es NULL! Reintentando búsqueda...");
            FindPlayerAttack();
        }
        
        if (playerAttack != null)
        {
            playerAttack.Atacar();
            Debug.Log("[AttackButton] Atacar() ejecutado");
        }
        else
        {
            Debug.LogError("[AttackButton] Todavía no se puede atacar - PlayerAttack no encontrado");
        }
    }
    
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnAttackClicked);
        }
    }
}