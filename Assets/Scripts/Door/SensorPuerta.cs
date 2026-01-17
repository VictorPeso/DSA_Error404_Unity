using UnityEngine;

public class SensorPuerta : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 3f;      // Qué tan rápido baja
    public float distanciaBajar = 4f; // Cuántos metros baja (para esconderse en el suelo)

    private Vector3 posicionFinal;
    private bool abriendo = false;

    void Start()
    {
        // Calculamos dónde tiene que terminar la puerta (4 metros más abajo)
        posicionFinal = transform.position - new Vector3(0, distanciaBajar, 0);
    }

    void Update()
    {
        // Si se activó la señal de abrir, movemos la puerta poco a poco
        if (abriendo)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionFinal, velocidad * Time.deltaTime);

            // Opcional: Si ya llegó abajo del todo, apagamos el script para ahorrar batería
            if (transform.position == posicionFinal)
            {
                enabled = false;
            }
        }
    }

    // ESTO ES EL SENSOR
    // Se activa cuando algo entra en la caja invisible (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si lo que entró es el Jugador (para que no la abran los enemigos)
        if (other.CompareTag("Player"))
        {
            abriendo = true;
        }
    }
}