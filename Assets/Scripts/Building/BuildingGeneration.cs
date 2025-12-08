using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGeneration : MonoBehaviour
{
    [SerializeField] private GameObject BuildPreview;
    [SerializeField] private Button BuildButton;
    [SerializeField] private Material BuildPreviewMat;
    [SerializeField] private Material BuildMat;

    private int i = 0;
    private GameObject previewInstance;

    public void SpawnBuilding()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;

        if (tiles.Count == 0) return;

        if (previewInstance == null)
        {
            Vector3 spawnPos = tiles[i].transform.position + new Vector3(0, 1f, 0);
            previewInstance = Instantiate(BuildPreview, spawnPos, Quaternion.identity);

            previewInstance.GetComponent<Renderer>().material = BuildPreviewMat;
            BuildButton.gameObject.SetActive(false);
        }
    }

    public void NextPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (previewInstance == null || tiles.Count == 0) return;

        i = (i + 1) % tiles.Count;
        previewInstance.transform.position = tiles[i].transform.position + new Vector3(0, 1f, 0);
    }

    public void PrevPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (previewInstance == null || tiles.Count == 0) return;

        i = (i - 1 + tiles.Count) % tiles.Count;
        previewInstance.transform.position = tiles[i].transform.position + new Vector3(0, 1f, 0);
    }

    public void ValidateSpawn()
    {
        if (previewInstance != null)
        {
            List<Tile> tiles = TileManager.Instance.freeTiles;

            previewInstance.GetComponent<Renderer>().material = BuildMat;

            // On retire la tuile globale
            TileManager.Instance.RemoveTile(tiles[i]);

            previewInstance = null;
            BuildButton.gameObject.SetActive(true);
        }
    }
}
