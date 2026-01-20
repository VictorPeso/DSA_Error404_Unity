using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    [Header("Laberinto")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 2f;

    [Header("Referencias de Construcción")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public NavMeshSurface navMeshSurface;

    [Header("Enemigos")]
    public GameObject enemyPrefab;
    [Range(0, 100)] public int enemyPercentage = 10;

    [Header("Boss")]
    public GameObject doorPrefab;
    public Transform doorTarget;
    public float ajusteAlturaPuerta = 1.0f;
    public float rotacionExtraSalida = 0f;

    [Header("Entrada")]
    public Transform startTarget;
    public GameObject entranceDoorPrefab;
    [Range(1, 10)] public int anchoEntrada = 3;
    public float rotacionExtraEntrada = 0f;
    public float offsetMuroEntrada = 2.0f;

    private int[,] maze;
    private List<Vector3> emptyCells = new List<Vector3>();

    void Start()
    {
        maze = new int[width, height];
        InitializeMaze();

        int startX = 0;
        int startY = 0;
        if (startTarget != null)
        {
            Vector3 localPos = startTarget.position - transform.position;
            startX = Mathf.FloorToInt((localPos.x + (width * cellSize) / 2f) / cellSize);
            startY = Mathf.FloorToInt((localPos.z + (height * cellSize) / 2f) / cellSize);
            startX = Mathf.Clamp(startX, 0, width - 1);
            startY = Mathf.Clamp(startY, 0, height - 1);
        }

        int targetX = -1;
        int targetY = -1;
        if (doorTarget != null)
        {
            Vector3 localPosDoor = doorTarget.position - transform.position;
            targetX = Mathf.FloorToInt((localPosDoor.x + (width * cellSize) / 2f) / cellSize);
            targetY = Mathf.FloorToInt((localPosDoor.z + (height * cellSize) / 2f) / cellSize);
            targetX = Mathf.Clamp(targetX, 0, width - 1);
            targetY = Mathf.Clamp(targetY, 0, height - 1);
        }

        GenerateMaze(startX, startY);

        maze[startX, startY] = 0;


        if (startY == 0 || startY == height - 1)
        {
            if (startX > 0) maze[startX - 1, startY] = 0;
            if (startX < width - 1) maze[startX + 1, startY] = 0;

            int dirY = (startY == 0) ? 1 : -1;
            maze[startX, startY + dirY] = 0;
        }
        else
        {
            if (startY > 0) maze[startX, startY - 1] = 0;
            if (startY < height - 1) maze[startX, startY + 1] = 0;

            int dirX = (startX == 0) ? 1 : -1;
            maze[startX + dirX, startY] = 0;
        }
        // --------------------------------------

        if (targetX != -1) maze[targetX, targetY] = 0;

        SpawnBossDoor();

        BuildPerimeter(startX, startY, targetX, targetY);

        SpawnEntranceDoor(startX, startY);

        BuildDungeon();
        if (navMeshSurface != null) navMeshSurface.BuildNavMesh();
        SpawnEnemies();
    }

    void InitializeMaze()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;
    }

    void GenerateMaze(int x, int y)
    {
        maze[x, y] = 0;
        List<int> directions = new List<int> { 0, 1, 2, 3 };
        Shuffle(directions);

        foreach (int dir in directions)
        {
            int nx = x, ny = y;
            switch (dir)
            {
                case 0: ny += 2; break;
                case 1: nx += 2; break;
                case 2: ny -= 2; break;
                case 3: nx -= 2; break;
            }

            if (nx >= 0 && nx < width && ny >= 0 && ny < height && maze[nx, ny] == 1)
            {
                maze[(x + nx) / 2, (y + ny) / 2] = 0;
                maze[nx, ny] = 0;
                GenerateMaze(nx, ny);
            }
        }
    }

    void BuildDungeon()
    {
        if (floorPrefab != null)
        {
            float totalWidth = width * cellSize;
            float totalLength = height * cellSize;

            GameObject hugeFloor = Instantiate(floorPrefab, transform.position, Quaternion.identity, transform);

            hugeFloor.transform.localScale = new Vector3(totalWidth, 0.1f, totalLength);

            hugeFloor.name = "Suelo_Gigante";
        }

        float startX = -(width * cellSize) / 2f;
        float startZ = -(height * cellSize) / 2f;
        Vector3 offset = new Vector3(cellSize / 2f, 0, cellSize / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 1)
                {
                    Vector3 pos = new Vector3(startX + (x * cellSize), 0, startZ + (y * cellSize)) + offset;
                    Vector3 worldPos = transform.position + pos;

                    GameObject wall = Instantiate(wallPrefab, worldPos, Quaternion.identity, transform);

                    wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, cellSize);
                }
                else
                {
                    Vector3 pos = new Vector3(startX + (x * cellSize), 0, startZ + (y * cellSize)) + offset;
                    Vector3 worldPos = transform.position + pos;
                    emptyCells.Add(worldPos);
                }
            }
        }
    }

    void SpawnEnemies()
    {
        if (emptyCells.Count == 0 || enemyPrefab == null) return;

        int safeZone = 3;

        for (int i = safeZone; i < emptyCells.Count - safeZone; i++)
        {
            if (Random.Range(0, 100) < enemyPercentage)
            {
                Instantiate(enemyPrefab, emptyCells[i], Quaternion.identity);
                GameObject enemy = Instantiate(enemyPrefab, emptyCells[i], Quaternion.identity);
                enemy.SetActive(true); // Asegurar que el enemigo esté activo
            }
        }
    }

    void SpawnEntranceDoor(int x, int y)
    {
        if (entranceDoorPrefab == null) return;

        float startMapX = -(width * cellSize) / 2f;
        float startMapZ = -(height * cellSize) / 2f;
        Vector3 offsetCentro = new Vector3(cellSize / 2f, 0, cellSize / 2f);

        Vector3 finalPos = new Vector3(startMapX + (x * cellSize), 0, startMapZ + (y * cellSize)) + offsetCentro;

        Quaternion rotation = Quaternion.Euler(0, rotacionExtraEntrada, 0);
        Vector3 empuje = Vector3.zero;

        if (y == 0)
        {
            empuje = new Vector3(0, 0, -offsetMuroEntrada);
        }
        else if (y == height - 1)
        {
            empuje = new Vector3(0, 0, offsetMuroEntrada);
        }
        else if (x == 0)
        {
            empuje = new Vector3(-offsetMuroEntrada, 0, 0);
        }
        else
        {
            empuje = new Vector3(offsetMuroEntrada, 0, 0);
        }

        Vector3 posDefinitiva = transform.position + finalPos + empuje;

        Instantiate(entranceDoorPrefab, posDefinitiva, rotation, transform);
    }

    void SpawnBossDoor()
    {
        if (doorTarget == null) return;

        Vector3 localPos = doorTarget.position - transform.position;
        float totalWidth = width * cellSize;
        float totalHeight = height * cellSize;
        int targetX = Mathf.FloorToInt((localPos.x + (totalWidth / 2f)) / cellSize);
        int targetY = Mathf.FloorToInt((localPos.z + (totalHeight / 2f)) / cellSize);
        targetX = Mathf.Clamp(targetX, 0, width - 1);
        targetY = Mathf.Clamp(targetY, 0, height - 1);

        maze[targetX, targetY] = 0;
        if (targetX > 0) maze[targetX - 1, targetY] = 0;
        else if (targetY > 0) maze[targetX, targetY - 1] = 0;

        if (doorPrefab != null)
        {
            float startX = -(width * cellSize) / 2f;
            float startZ = -(height * cellSize) / 2f;
            Vector3 offset = new Vector3(cellSize / 2f, 0, cellSize / 2f);

            Vector3 finalPos = new Vector3(startX + (targetX * cellSize), ajusteAlturaPuerta, startZ + (targetY * cellSize)) + offset;
            Quaternion rotacion = Quaternion.Euler(0, rotacionExtraSalida, 0);

            Instantiate(doorPrefab, transform.position + finalPos, rotacion, transform);
        }
    }

    void BuildPerimeter(int startX, int startY, int doorX, int doorY)
    {
        float startMapX = -(width * cellSize) / 2f;
        float startMapZ = -(height * cellSize) / 2f;
        Vector3 offset = new Vector3(cellSize / 2f, 0, cellSize / 2f);

        for (int y = 0; y < height; y++)
        {
            bool esRangoEntrada = (Mathf.Abs(startY - y) <= anchoEntrada / 2);

            bool huecoEntradaIzq = (startX == 0 && esRangoEntrada);
            bool huecoSalidaIzq = (doorX == 0 && doorY == y);

            if (!huecoEntradaIzq && !huecoSalidaIzq)
            {
                Vector3 pos = new Vector3(startMapX - cellSize, 0, startMapZ + (y * cellSize)) + offset;
                Instantiate(wallPrefab, transform.position + pos, Quaternion.identity, transform);
            }

            bool huecoEntradaDer = (startX == width - 1 && esRangoEntrada);
            bool huecoSalidaDer = (doorX == width - 1 && doorY == y);

            if (!huecoEntradaDer && !huecoSalidaDer)
            {
                Vector3 pos = new Vector3(startMapX + (width * cellSize), 0, startMapZ + (y * cellSize)) + offset;
                Instantiate(wallPrefab, transform.position + pos, Quaternion.identity, transform);
            }
        }

        for (int x = 0; x < width; x++)
        {
            bool esRangoEntrada = (Mathf.Abs(startX - x) <= anchoEntrada / 2);

            bool huecoEntradaAbajo = (startY == 0 && esRangoEntrada);
            bool huecoSalidaAbajo = (doorY == 0 && doorX == x);

            if (!huecoEntradaAbajo && !huecoSalidaAbajo)
            {
                Vector3 pos = new Vector3(startMapX + (x * cellSize), 0, startMapZ - cellSize) + offset;
                Instantiate(wallPrefab, transform.position + pos, Quaternion.Euler(0, 90, 0), transform);
            }

            bool huecoEntradaArriba = (startY == height - 1 && esRangoEntrada);
            bool huecoSalidaArriba = (doorY == height - 1 && doorX == x);

            if (!huecoEntradaArriba && !huecoSalidaArriba)
            {
                Vector3 pos = new Vector3(startMapX + (x * cellSize), 0, startMapZ + (height * cellSize)) + offset;
                Instantiate(wallPrefab, transform.position + pos, Quaternion.Euler(0, 90, 0), transform);
            }
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 size = new Vector3(width * cellSize, 1, height * cellSize);
        Gizmos.DrawWireCube(transform.position, size);
        if (doorTarget != null)
        {
            Gizmos.color = Color.red;

            Vector3 localPos = doorTarget.position - transform.position;
            int tx = Mathf.FloorToInt((localPos.x + (width * cellSize / 2f)) / cellSize);
            int ty = Mathf.FloorToInt((localPos.z + (height * cellSize / 2f)) / cellSize);
            tx = Mathf.Clamp(tx, 0, width - 1);
            ty = Mathf.Clamp(ty, 0, height - 1);

            Vector3 offset = new Vector3(cellSize / 2f, 0, cellSize / 2f);
            float sX = -(width * cellSize) / 2f;
            float sZ = -(height * cellSize) / 2f;

            Vector3 dPos = transform.position + new Vector3(sX + (tx * cellSize), 0, sZ + (ty * cellSize)) + offset;

            Gizmos.DrawWireCube(dPos, new Vector3(cellSize, cellSize, cellSize));
            Gizmos.DrawLine(doorTarget.position, dPos);
        }

        if (startTarget != null)
        {
            Gizmos.color = Color.green;

            Vector3 localPos = startTarget.position - transform.position;
            int sx = Mathf.FloorToInt((localPos.x + (width * cellSize / 2f)) / cellSize);
            int sy = Mathf.FloorToInt((localPos.z + (height * cellSize / 2f)) / cellSize);
            sx = Mathf.Clamp(sx, 0, width - 1);
            sy = Mathf.Clamp(sy, 0, height - 1);

            Vector3 offset = new Vector3(cellSize / 2f, 0, cellSize / 2f);
            float mapStartX = -(width * cellSize) / 2f;
            float mapStartZ = -(height * cellSize) / 2f;

            Vector3 entPos = transform.position + new Vector3(mapStartX + (sx * cellSize), 0, mapStartZ + (sy * cellSize)) + offset;

            Gizmos.DrawWireCube(entPos, new Vector3(cellSize * anchoEntrada, 2, cellSize));
            Gizmos.DrawLine(startTarget.position, entPos);
        }
    }
}