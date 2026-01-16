using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    [Header("Configuración del Laberinto")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 2f;

    [Header("Referencias de Construcción")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public NavMeshSurface navMeshSurface;

    [Header("Configuración de Enemigos")]
    public GameObject enemyPrefab;
    [Range(0, 100)] public int enemyPercentage = 10;

    [Header("Boss Room Settings")]
    public GameObject doorPrefab;
    public Transform doorTarget;
    public float ajusteAlturaPuerta = 1.0f;

    private int[,] maze;
    private List<Vector3> emptyCells = new List<Vector3>();

    void Start()
    {
        maze = new int[width, height];
        InitializeMaze();
        GenerateMaze(0, 0);

        CreateDoorOnly();

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
                    // Guardamos posición para enemigos
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
            }
        }
    }

    void CreateDoorOnly()
    {
        if (doorTarget == null) return;

        Vector3 localPos = doorTarget.position - transform.position;
        float totalWidth = width * cellSize;
        float totalHeight = height * cellSize;

        int targetX = Mathf.FloorToInt((localPos.x + (totalWidth / 2f)) / cellSize);
        int targetY = Mathf.FloorToInt((localPos.z + (totalHeight / 2f)) / cellSize);

        targetX = Mathf.Clamp(targetX, 1, width - 2); // Evitamos bordes extremos para simplificar
        targetY = Mathf.Clamp(targetY, 1, height - 2);

        maze[targetX, targetY] = 0;

        Quaternion doorRotation = Quaternion.identity;

        bool pasilloHorizontal = (maze[targetX - 1, targetY] == 0 || maze[targetX + 1, targetY] == 0);
        bool pasilloVertical = (maze[targetX, targetY - 1] == 0 || maze[targetX, targetY + 1] == 0);

        if (pasilloHorizontal && !pasilloVertical)
        {
            doorRotation = Quaternion.Euler(0, 90, 0);
            if (targetX > 0) maze[targetX - 1, targetY] = 0;
            if (targetX < width - 1) maze[targetX + 1, targetY] = 0;
        }
        else
        {
            if (targetY > 0) maze[targetX, targetY - 1] = 0;
            if (targetY < height - 1) maze[targetX, targetY + 1] = 0;
        }

        if (doorPrefab != null)
        {
            float startX = -(width * cellSize) / 2f;
            float startZ = -(height * cellSize) / 2f;
            Vector3 offset = new Vector3(cellSize / 2f, 0, cellSize / 2f);

            Vector3 finalPos = new Vector3(startX + (targetX * cellSize), ajusteAlturaPuerta, startZ + (targetY * cellSize)) + offset;

            GameObject door = Instantiate(doorPrefab, transform.position + finalPos, doorRotation, transform);
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
            float totalW = width * cellSize;
            float totalH = height * cellSize;
            int tx = Mathf.FloorToInt((localPos.x + (totalW / 2f)) / cellSize);
            int ty = Mathf.FloorToInt((localPos.z + (totalH / 2f)) / cellSize);

            tx = Mathf.Clamp(tx, 0, width - 1);
            ty = Mathf.Clamp(ty, 0, height - 1);

            float sX = -(width * cellSize) / 2f;
            float sZ = -(height * cellSize) / 2f;
            Vector3 off = new Vector3(cellSize / 2f, 0, cellSize / 2f);
            Vector3 dPos = transform.position + new Vector3(sX + (tx * cellSize), 0, sZ + (ty * cellSize)) + off;

            Gizmos.DrawWireCube(dPos, new Vector3(cellSize, cellSize, cellSize));
            Gizmos.DrawLine(doorTarget.position, dPos);
        }
    }
}