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
    private GameObject navButton;

    [SerializeField]
    private Camera Secondcam;


    public void Awake()
    {
        Secondcam.gameObject.SetActive(false);
        navButton.gameObject.SetActive(false);

    }

    public void SetBuilding(BuildingData building)
    {
        currentBuilding = building;
        SpawnBuilding();
    }

    public void SpawnBuilding()
    {
        navButton.gameObject.SetActive(true);
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

        // --- MISE À JOUR D'UNE PRÉVISUALISATION EXISTANTE ---
        if (previewInstance != null)
        {
            // Utiliser GetComponentInChildren pour chercher sur l'objet ou ses enfants (Correct)
            Renderer buildingRenderer = previewInstance.GetComponentInChildren<Renderer>();

            if (buildingRenderer != null)
            {
                // Ligne 59 (sécurisée)
                buildingRenderer.material = currentBuilding.previewMaterial;
            }
            else
            {
                Debug.LogError("Renderer introuvable sur le GameObject de prévisualisation ou ses enfants lors du Spawn.");
            }
        }

        if (previewInstance == null)
        {
            Debug.Log("normalement c'est good");
            Maincam.gameObject.SetActive(false);
            Secondcam.gameObject.SetActive(true);


            int tileIndex = 0; 
            if (tiles.Count > 0)
            {
                tileIndex = i; 
            }

            Vector3 spawnPos = tiles[tileIndex].transform.position;
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            previewInstance = Instantiate(currentBuilding.prefab, spawnPos, randomRotation);

            Renderer newBuildingRenderer = previewInstance.GetComponentInChildren<Renderer>();

            if (newBuildingRenderer != null)
            {
                newBuildingRenderer.material = currentBuilding.previewMaterial;
            }
            else
            {
                Debug.LogError("Renderer introuvable sur la nouvelle instance de prévisualisation ou ses enfants.");
            }
        }
    }

    public void NextPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0 || previewInstance == null) return;

        i = Random.Range(0, tiles.Count);

        previewInstance.transform.position = tiles[i].transform.position;
    }

    public void PrevPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0 || previewInstance == null) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, tiles.Count);
        } while (tiles.Count > 1 && newIndex == i);

        i = newIndex;

        previewInstance.transform.position = tiles[i].transform.position;
    }


    public void ValidateSpawn()
    {
        navButton.gameObject.SetActive(false);
        Debug.Log(currentBuilding.buildingName + " construit !");

        if (previewInstance != null)
        {
            Renderer buildingRenderer = previewInstance.GetComponentInChildren<Renderer>();

            if (buildingRenderer != null)
            {
                buildingRenderer.material = currentBuilding.finalMaterial;

                List<Tile> tiles = TileManager.Instance.freeTiles;
                TileManager.Instance.RemoveTile(tiles[i]);

                previewInstance = null;
                Maincam.gameObject.SetActive(true);
                Secondcam.gameObject.SetActive(false);
            }
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


