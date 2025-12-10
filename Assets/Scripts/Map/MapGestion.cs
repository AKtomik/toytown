using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;

[ExecuteAlways]
public class HexGridPrefabSpawner : MonoBehaviour
{
    public GameObject[] tilesPrefabs;

    // Rayon pour la forme hexagonale (1 = 7 hex, 2 = 19 hex, 3 = 37 hex, etc.)
    public int hexRadius = 3;

    [Header("Probabilité Spécifique")]
    public string boostedTileTag = "Plain";

    [Range(1f, 10f)]
    public float boostMultiplier = 2.0f;

    private Grid grid;
    private Tilemap tilemap;
    private Transform spawnRoot;

    public float rotationAngleY = 25f;

    // Vérifie si une cellule fait partie de la forme hexagonale RONDE
    private bool IsValidHexCell(int x, int y)
    {
        // Pour offset coordinates (odd-r ou even-r)
        // Conversion vers axial coordinates (q, r)
        int q = x - (y - (y & 1)) / 2;
        int r = y;

        // Conversion vers cube coordinates
        int s = -q - r;

        // Distance cube = max(|q|, |r|, |s|)
        int distance = Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s));

        return distance <= hexRadius;
    }

    private GameObject ChooseWeightedPrefab()
    {
        if (tilesPrefabs == null || tilesPrefabs.Length == 0)
        {
            return null;
        }

        List<GameObject> weightedList = new List<GameObject>();

        foreach (var prefab in tilesPrefabs)
        {
            if (prefab == null) continue;

            float weight = 1f;

            if (prefab.CompareTag(boostedTileTag))
            {
                weight = boostMultiplier;
            }

            for (int i = 0; i < (int)weight; i++)
            {
                weightedList.Add(prefab);
            }
        }

        if (weightedList.Count == 0) return null;

        int index = Random.Range(0, weightedList.Count);
        return weightedList[index];
    }

    public void GenerateGrid()
    {
        grid = GetComponent<Grid>();
        tilemap = GetComponentInChildren<Tilemap>();

        if (grid == null)
        {
            Debug.LogError("Erreur : Le composant Grid est manquant sur ce GameObject.");
            return;
        }

        if (tilemap == null)
        {
            Debug.LogError("Erreur : Le composant Tilemap (enfant) est manquant.");
            return;
        }

        if (spawnRoot == null)
        {
            Transform exists = transform.Find("SpawnRoot");
            if (exists != null) spawnRoot = exists;
            else
            {
                GameObject sr = new GameObject("SpawnRoot");
                sr.transform.SetParent(transform);
                sr.transform.localPosition = Vector3.zero;
                spawnRoot = sr.transform;
            }
        }

        List<Transform> toDestroy = new List<Transform>();
        foreach (Transform child in spawnRoot) toDestroy.Add(child);

#if UNITY_EDITOR
        foreach (var c in toDestroy) DestroyImmediate(c.gameObject);
#else
        foreach (var c in toDestroy) Destroy(c.gameObject);
#endif

        Quaternion rotation = Quaternion.Euler(0f, rotationAngleY, 0f);

        int tilesGenerated = 0;

        // Calculer la zone à parcourir
        int range = hexRadius * 2 + 1;

        // Génération hexagonale RONDE
        for (int y = -range; y <= range; y++)
        {
            for (int x = -range; x <= range; x++)
            {
                if (!IsValidHexCell(x, y))
                    continue;

                Vector3Int cell = new Vector3Int(x, y, 0);
                Vector3 worldPos = grid.GetCellCenterWorld(cell);

                GameObject prefab = ChooseWeightedPrefab();

                if (prefab == null)
                    continue;

                GameObject go = Instantiate(prefab, worldPos, rotation, spawnRoot);

                MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();
                if (mr != null)
                {
                    Vector3 meshSize = mr.bounds.size;
                    float cellWidth = grid.cellSize.x;
                    float uniformScale = cellWidth / meshSize.x;
                    go.transform.localScale = Vector3.one * uniformScale;
                }

                tilesGenerated++;
            }
        }

        int expectedTiles = 1 + 3 * hexRadius * (hexRadius + 1);
        Debug.Log($"Génération terminée : {tilesGenerated} tuiles (attendu: {expectedTiles})");

        if (tilesGenerated != expectedTiles)
        {
            Debug.LogWarning($" Nombre de tuiles : {tilesGenerated} (attendu: {expectedTiles})");
        }
    }

    void Start()
    {
        if (Application.isPlaying)
            GenerateGrid();
    }
}