using UnityEngine;

public class CamaraBloqueada : MonoBehaviour
{
    [Header("ARRASTRA AQUÍ AL JUGADOR")]
    public Transform objetivo;

    // Variables privadas para guardar TU configuración
    private Vector3 distanciaInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        if (objetivo == null)
        {
            Debug.LogError("¡FALTA EL JUGADOR! Arrástralo a la casilla Objetivo.");
            return;
        }

        // 1. MEMORIZAR: Guardamos exactamente dónde pusiste la cámara en la escena
        // Si la pusiste a Rotación 80, guardará 80. Si la pusiste lejos, guardará esa distancia.
        distanciaInicial = transform.position - objetivo.position;
        rotacionInicial = transform.rotation;
    }

    // LateUpdate ocurre AL FINAL de todo. Es el "Jefe" que decide la posición final.
    void LateUpdate()
    {
        if (objetivo != null)
        {
            // 2. SEGUIR: Moverse con el jugador manteniendo la distancia original
            transform.position = objetivo.position + distanciaInicial;

            // 3. BLOQUEAR: Forzar la rotación original.
            // Esto anula cualquier movimiento de ratón o dedo que intente girarla.
            transform.rotation = rotacionInicial;
        }
    }
}