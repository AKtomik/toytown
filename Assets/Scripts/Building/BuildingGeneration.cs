using System.Collections;
using System.Collections.Generic;
using ToyTown;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingGeneration : MonoBehaviour
{
    private BuildingData currentBuilding;
    private GameObject previewInstance;
    private int i = 0;

    [SerializeField]
    private Camera Maincam;

    [SerializeField]
    private GameObject navButton;

    private PlaceManager placeManager;

    [SerializeField]
    private Camera Secondcam;

    public void Awake()
    {
        Secondcam.gameObject.SetActive(false);
        navButton.gameObject.SetActive(false);
    }

    public void Start()
    {
        // Sécurité pour trouver le PlaceManager s'il n'est pas assigné
        if (PlaceManager.Instance != null)
        {
            placeManager = PlaceManager.Instance;
        }
    }

    public void SetBuilding(BuildingData building)
    {
        currentBuilding = building;
        SpawnBuilding();
    }

    public void SpawnBuilding()
    {
        navButton.gameObject.SetActive(true);

        // On récupère la liste à jour (les tuiles en attente de construction n'y sont plus)
        List<Tile> tiles = TileManager.Instance.freeTiles;

        if (!VerifyResources())
        {
            Debug.Log("Pas assez de ressources");
            return;
        }

        // Mise à jour visuelle si on change juste de bâtiment
        if (previewInstance != null)
        {
            Renderer buildingRenderer = previewInstance.GetComponentInChildren<Renderer>();
            if (buildingRenderer != null)
            {
                buildingRenderer.material = currentBuilding.previewMaterial;
            }
        }

        // Création d'une nouvelle preview si aucune n'existe
        if (previewInstance == null)
        {
            Maincam.gameObject.SetActive(false);
            Secondcam.gameObject.SetActive(true);

            // On s'assure que l'index i est valide
            if (tiles.Count > 0)
            {
                // Si l'index i dépasse la nouvelle taille de liste, on le remet à 0
                if (i >= tiles.Count) i = 0;

                Vector3 spawnPos = tiles[i].transform.position;
                Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                previewInstance = Instantiate(currentBuilding.prefab, spawnPos, randomRotation);

                Renderer newBuildingRenderer = previewInstance.GetComponentInChildren<Renderer>();

                Renderer[] allRenderers = previewInstance.GetComponentsInChildren<Renderer>();

                if (allRenderers != null && allRenderers.Length > 0)
                {
                    // Parcourt tous les Renderers trouvés sur l'objet parent et ses enfants
                    foreach (Renderer renderer in allRenderers)
                    {
                        // Applique le nouveau matériau à ce Renderer
                        renderer.material = currentBuilding.previewMaterial;
                    }
                }
            }
            else
            {
                Debug.Log("Aucune tuile libre disponible !");
                navButton.gameObject.SetActive(false);
            }
        }
    }

    public void NextPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0 || previewInstance == null) return;

        // On prend une nouvelle position aléatoire
        i = Random.Range(0, tiles.Count);
        previewInstance.transform.position = tiles[i].transform.position;
    }

    public void PrevPos()
    {
        List<Tile> tiles = TileManager.Instance.freeTiles;
        if (tiles.Count == 0 || previewInstance == null) return;

        int newIndex;
        // Petit algo pour essayer d'avoir une position différente de l'actuelle
        int attempts = 0;
        do
        {
            newIndex = Random.Range(0, tiles.Count);
            attempts++;
        } while (tiles.Count > 1 && newIndex == i && attempts < 10);

        i = newIndex;
        previewInstance.transform.position = tiles[i].transform.position;
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

    public void StartBuild()
    {
        if (previewInstance == null) return;

        navButton.gameObject.SetActive(false);
        Maincam.gameObject.SetActive(true);
        Secondcam.gameObject.SetActive(false);

        List<Tile> tiles = TileManager.Instance.freeTiles;

        // Vérification de sécurité
        if (tiles.Count == 0) return;
        if (i >= tiles.Count) i = 0; // Sécurité si l'index est hors limite

        Tile selectedTile = tiles[i];
        GameObject buildingToConstruct = previewInstance;
        BuildingData buildingData = currentBuilding;

        // --- CHANGEMENT ICI ---
        // On retire la tuile de la liste MAINTENANT.
        // Comme ça, le prochain SpawnBuilding() ne pourra pas la sélectionner.
        TileManager.Instance.RemoveTile(selectedTile);

        // On détache la preview du script pour pouvoir en relancer une autre
        previewInstance = null;

        // On lance la construction autonome
        StartCoroutine(ConstructionRoutine(selectedTile, buildingToConstruct, buildingData));
    }

    private IEnumerator ConstructionRoutine(Tile targetTile, GameObject buildingInstance, BuildingData data)
    {
        Debug.Log($"En attente du BUILDER sur la tuile {targetTile.name} pour {data.buildingName}");

        // Attente du builder (nécessite que Tile.cs ait IsBuilderPresent)
        while (targetTile == null || !targetTile.IsBuilderPresent)
        {
            yield return null;
        }

        Debug.Log($"Builder détecté sur {targetTile.name} ! Construction de {data.buildingName} en cours...");

        yield return new WaitForSeconds(5);

        Debug.Log($"Construction de {data.buildingName} terminée !");
        FinalizeConstruction(buildingInstance, data);
    }

    public void FinalizeConstruction(GameObject buildingInstance, BuildingData data)
    {
        if (buildingInstance == null) return;

        // Changement visuel final
        Renderer buildingRenderer = buildingInstance.GetComponentInChildren<Renderer>();
        if (buildingRenderer != null)
        {
            buildingRenderer.material = data.finalMaterial;
        }

        if (placeManager != null && placeManager.PlaceDictionary.ContainsKey(data.associatedPlace))
        {
            placeManager.PlaceDictionary[data.associatedPlace].Add(buildingInstance);
        }
    }
}