using UnityEngine;

public class CamaraBloqueada : MonoBehaviour
{
    [Header("JUGADOR")]
    public Transform objetivo;

    private Vector3 distanciaInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        if (objetivo == null)
        {
            Debug.LogError("¡FALTA EL JUGADOR! Arrástralo a la casilla Objetivo.");
            return;
        }

        distanciaInicial = transform.position - objetivo.position;
        rotacionInicial = transform.rotation;
    }

    void LateUpdate()
    {
        if (objetivo != null)
        {
            transform.position = objetivo.position + distanciaInicial;
            transform.rotation = rotacionInicial;
        }
    }
}