using UnityEngine;
using UnityEngine.UIElements;

public class ShopController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement potion;   
    private Button buyButton;       
    private Label coinLabel;        
    private int playerCoins = 100;  

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        potion = root.Q<VisualElement>("Potion");
        buyButton = root.Q<Button>(); 
        coinLabel = root.Q<Label>("Coin");

        UpdateCoinLabel();

        if (buyButton != null)
            buyButton.clicked += OnBuyPotion;
    }

    void OnDisable()
    {
        if (buyButton != null)
            buyButton.clicked -= OnBuyPotion;
    }

    private void OnBuyPotion()
    {
        int potionCost = 20;

        if (playerCoins >= potionCost)
        {
            playerCoins -= potionCost;
            UpdateCoinLabel();
            Debug.Log("Poción comprada");
        }
        else
        {
            Debug.Log("No tienes suficientes monedas.");
        }
    }

    private void UpdateCoinLabel()
    {
        if (coinLabel != null)
            coinLabel.text = "Coin: " + playerCoins;
    }
}
