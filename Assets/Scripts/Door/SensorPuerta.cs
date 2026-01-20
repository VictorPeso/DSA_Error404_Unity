using UnityEngine;

public class SensorPuerta : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 3f;
    public float distanciaBajar = 4f;

    private Vector3 posicionFinal;
    private bool abriendo = false;

    void Start()
    {
        posicionFinal = transform.position - new Vector3(0, distanciaBajar, 0);
    }

    void Update()
    {
        if (abriendo)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionFinal, velocidad * Time.deltaTime);

            if (transform.position == posicionFinal)
            {
                enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            abriendo = true;
        }
    }
}