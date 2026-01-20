using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    public Transform playerTransform;
    public float arrowHeight = 2f;
    public float arrowDistance = 1.5f;
    public float rotationSpeed = 10f;
    
    private Quaternion targetRotation;
    private Transform arrowTransform;
    
    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        arrowTransform = transform;
    }
    
    void LateUpdate()
    {
        if (playerTransform == null) return;
        
        Vector3 targetPosition = playerTransform.position + Vector3.up * arrowHeight;
        arrowTransform.position = targetPosition;
        
        targetRotation = Quaternion.LookRotation(playerTransform.forward);
        arrowTransform.rotation = Quaternion.Slerp(arrowTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}