using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingGeneration : MonoBehaviour
{
    //[SerializeField] private Button BuildButton;

    private BuildingData currentBuilding;    // Le bâtiment sélectionné
    private GameObject previewInstance;
    private int i = 0;
    [SerializeField]
    private Camera Maincam;

    [SerializeField]
    private Camera Secondcam;


    public void Awake()
    {
        Secondcam.gameObject.SetActive(false);
    }

    public void SetBuilding(BuildingData building)
    {
        currentBuilding = building;
        SpawnBuilding();
    }

    public void SpawnBuilding()
    {

        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0)
        {
            Debug.Log("pas de tile detecte");
            return;
        }

        if (!VerifyResources())
        {
            Debug.Log("Pas assez de ressources");
            return;
        }

        if (previewInstance == null)
        {
            Debug.Log("normalement c'est good");
            Maincam.gameObject.SetActive(false);
            Secondcam.gameObject.SetActive(true);
            // Choisir une tile pour placer la preview
            Vector3 spawnPos = tiles[i].transform.position + Vector3.up;
            previewInstance = Instantiate(currentBuilding.prefab, spawnPos, Quaternion.identity);
            previewInstance.GetComponent<Renderer>().material = currentBuilding.previewMaterial;

        }
    }

    public void NextPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0 || previewInstance == null) return;

        // Choisir un index aléatoire
        i = Random.Range(0, tiles.Count);

        // Placer la preview sur la tile choisie
        previewInstance.transform.position = tiles[i].transform.position + Vector3.up;
    }

    public void PrevPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0 || previewInstance == null) return;

        // Choisir un index aléatoire différent si tu veux
        int newIndex;
        do
        {
            newIndex = Random.Range(0, tiles.Count);
        } while (tiles.Count > 1 && newIndex == i); // pour ne pas rester sur la même tile

        i = newIndex;

        previewInstance.transform.position = tiles[i].transform.position + Vector3.up;
    }


    public void ValidateSpawn()
    {
        /*if (currentBuilding == null || !VerifyResources())
        {
            Debug.Log("Pas assez de ressources");
            return;
        }*/

        Debug.Log(currentBuilding.buildingName + " construit !");

        if (previewInstance != null)
        {
            previewInstance.GetComponent<Renderer>().material = currentBuilding.finalMaterial;

            List<Tile> tiles = TileManager.Instance.freeTiles;
            TileManager.Instance.RemoveTile(tiles[i]);

            previewInstance = null;
            //BuildButton.gameObject.SetActive(true);
            Maincam.gameObject.SetActive(true); 
            Secondcam.gameObject.SetActive(false);

        }
    }

    private bool VerifyResources()
    {
        if (RessourcesGestion.RockQuantity >= currentBuilding.rockCost &&
            RessourcesGestion.WoodQuantity >= currentBuilding.woodCost)
        {
            RessourcesGestion.RemoveRock(currentBuilding.rockCost);
            RessourcesGestion.RemoveWood(currentBuilding.woodCost);
            return true;
        }

        return false;
    }
}


