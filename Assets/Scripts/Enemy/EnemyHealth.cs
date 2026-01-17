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
            Debug.LogError("[EnemyHealth] No Slider found in children");
        }
    }
    
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
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