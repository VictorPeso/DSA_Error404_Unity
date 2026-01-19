using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CanvasChecker : MonoBehaviour
{
    void Update()
    {
        Canvas canvas = GetComponent<Canvas>();
        
        if (canvas != null)
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Debug.LogWarning($"[CanvasChecker] {gameObject.name}: Canvas no está en ScreenSpaceOverlay. Móvil puede no funcionar correctamente.");
            }
        }
        
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        
        if (scaler != null)
        {
            if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                Debug.LogWarning($"[CanvasChecker] {gameObject.name}: CanvasScaler no está en ScaleWithScreenSize. Móvil puede no funcionar correctamente.");
            }
        }
        
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        
        if (raycaster == null)
        {
            Debug.LogWarning($"[CanvasChecker] {gameObject.name}: No tiene GraphicRaycaster. Los botones no funcionarán.");
        }
    }
}