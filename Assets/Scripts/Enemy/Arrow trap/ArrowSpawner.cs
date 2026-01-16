using UnityEngine;

public class ArrpwSpawner : MonoBehaviour
{
    public GameObject arrowPrefab; // Arrastra aquí tu prefab de flecha
    public Transform spawnPoint;   // Punto desde donde sale la flecha
    public float fireRate = 2f;    // Tiempo entre flechas
    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            SpawnArrow();
            nextFireTime = Time.time + fireRate;
        }
    }

    void SpawnArrow()
    {
        // Crea la flecha en la posición y rotación del lanzador
        Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
