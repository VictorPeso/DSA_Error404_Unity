using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();

        if (slider == null)
        {
            Debug.LogError("EnemyHealth: No se encontró Slider en los hijos");
        }
    }
    public void UpdateHealthBar(float currentVelue, float maxValue)
    {
        slider.value = currentVelue / maxValue;
    }

    void Update()
    {
        
    }

   
}