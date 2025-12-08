using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGeneration : MonoBehaviour
{
    //[SerializeField] private Button BuildButton;

    private BuildingData currentBuilding;    // Le bâtiment sélectionné
    private GameObject previewInstance;
    private int i = 0;

    public void SetBuilding(BuildingData building)
    {
        currentBuilding = building;
        SpawnBuilding();
    }

    public void SpawnBuilding()
    {
        if (currentBuilding == null) return;

        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0) return;

        if (previewInstance == null)
        {
            Vector3 spawnPos = tiles[i].transform.position + Vector3.up;
            previewInstance = Instantiate(currentBuilding.prefab, spawnPos, Quaternion.identity);
            previewInstance.GetComponent<Renderer>().material = currentBuilding.previewMaterial;

            //BuildButton.gameObject.SetActive(false);
        }
    }

    public void NextPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (previewInstance == null || tiles.Count == 0) return;

        i = (i + 1) % tiles.Count;
        previewInstance.transform.position = tiles[i].transform.position + Vector3.up;
    }

    public void PrevPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (previewInstance == null || tiles.Count == 0) return;

        i = (i - 1 + tiles.Count) % tiles.Count;
        previewInstance.transform.position = tiles[i].transform.position + Vector3.up;
    }

    public void ValidateSpawn()
    {
        if (currentBuilding == null || !VerifyResources())
        {
            Debug.Log("Pas assez de ressources");
            return;
        }

        Debug.Log(currentBuilding.buildingName + " construit !");

        if (previewInstance != null)
        {
            previewInstance.GetComponent<Renderer>().material = currentBuilding.finalMaterial;

            List<Tile> tiles = TileManager.Instance.freeTiles;
            TileManager.Instance.RemoveTile(tiles[i]);

            previewInstance = null;
            //BuildButton.gameObject.SetActive(true);
        }
    }

    private bool VerifyResources()
    {
        if (RessourcesGestion.rockQuantity >= currentBuilding.rockCost &&
            RessourcesGestion.woodQuantity >= currentBuilding.woodCost)
        {
            RessourcesGestion.RemoveRock(currentBuilding.rockCost);
            RessourcesGestion.RemoveWood(currentBuilding.woodCost);
            return true;
        }

        return false;
    }
}
