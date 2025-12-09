using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[ExecuteAlways]
public class GridPrefabSpawner : MonoBehaviour
{
    public GameObject[] tilesPrefabs; // tableau de prefabs pour tes tiles
    public int width = 10;
    public int height = 10;

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        Grid grid = GetComponent<Grid>();
        if (grid == null)
        {
            Debug.LogError("Le script doit être sur le GameObject qui contient le component Grid.");
            return;
        }

        // Clear previous children
        List<Transform> toDestroy = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++) toDestroy.Add(transform.GetChild(i));
#if UNITY_EDITOR
        foreach (var t in toDestroy) DestroyImmediate(t.gameObject);
#else
        foreach (var t in toDestroy) Destroy(t.gameObject);
#endif

        // origine de grille
        Vector3 originLocal = grid.CellToLocalInterpolated(new Vector3Int(0, 0, 0));
        Vector3 cellXLocal = grid.CellToLocalInterpolated(new Vector3Int(1, 0, 0)) - originLocal;
        Vector3 cellYLocal = grid.CellToLocalInterpolated(new Vector3Int(0, 1, 0)) - originLocal;
        Vector3 normalLocal = Vector3.Cross(cellXLocal.normalized, cellYLocal.normalized).normalized;
        Vector3 normalWorld = transform.TransformDirection(normalLocal);

        for (int gx = 0; gx < width; gx++)
        {
            for (int gy = 0; gy < height; gy++)
            {
                if (tilesPrefabs == null || tilesPrefabs.Length == 0) continue;

                GameObject prefab = tilesPrefabs[Random.Range(0, tilesPrefabs.Length)];
                if (prefab == null) continue;

                Vector3 localPos = grid.CellToLocalInterpolated(new Vector3Int(gx, gy, 0));
                GameObject go = Instantiate(prefab, transform);
                go.transform.localPosition = localPos;
                go.transform.localScale = Vector3.one;

                // Ajouter Tile
                Tile tile = go.GetComponent<Tile>();
                if (tile == null)
                    tile = go.AddComponent<Tile>();

                // Ajouter uniquement si c’est une plaine
                if (prefab.CompareTag("Plain")) // <- utiliser prefab.tag et non go.tag si le tag est sur le prefab
                {
                    TileManager.Instance.AddTile(tile);
                }



                // Alignement
                Vector3 prefabUpWorld = go.transform.up;
                Quaternion alignUp = Quaternion.FromToRotation(prefabUpWorld, normalWorld);
                go.transform.rotation = alignUp * go.transform.rotation;
            }
        }

        Debug.Log("GridPrefabSpawner : génération terminée. Normal locale = " + normalLocal.ToString("F3"));
    }

    void Start()
    {
        if (Application.isPlaying)
            GenerateGrid();
    }
}
