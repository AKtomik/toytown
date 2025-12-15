using System.Collections;
using System.Collections.Generic;
using ToyTown;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class BuildingGeneration : MonoBehaviour
{
    public static BuildingGeneration Instance { get; private set; }
    private BuildingData currentBuilding;
   
    private GameObject previewInstance;
    private int i = 0;

    [SerializeField]
    private Camera Maincam;

    [SerializeField]
    private GameObject navButton;
    [SerializeField]
    private GameObject cantButton;

    private PlaceManager placeManager;

    [SerializeField]
    private Camera Secondcam;

    public void Awake()
    {
        if (Secondcam != null) Secondcam.gameObject.SetActive(false);
        else Debug.LogError("BuildingGeneration: Secondcam is NULL");

        if (navButton != null) navButton.gameObject.SetActive(false);
        else Debug.LogError("BuildingGeneration: navButton is NULL");

        if (cantButton != null) cantButton.gameObject.SetActive(false);
        else Debug.LogError("BuildingGeneration: cantButton is NULL");
    }

    public void Start()
    {
        Instance = this;

        // S�curit� pour trouver le PlaceManager s'il n'est pas assign�
        if (PlaceManager.Instance != null)
        {
            placeManager = PlaceManager.Instance;
        }
    }

    public void SetBuilding(BuildingData building)
    {
        if (building == null)
        {
            Debug.LogError("BuildingGeneration.SetBuilding called with NULL building");
            return;
        }

        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null; 
        }

        // 2. Initialiser et lancer la nouvelle prévisualisation
        currentBuilding = building;
        SpawnBuilding();
    }

    public void SpawnBuilding()
    {
        if (navButton != null) navButton.gameObject.SetActive(true);
        if (cantButton != null) cantButton.gameObject.SetActive(false);

        // On r�cup�re la liste � jour (les tuiles en attente de construction n'y sont plus)
        // Defensive: ensure TileManager exists and remove null entries which sometimes
        // appear only in builds (destroyed objects still referenced in lists)
        List<Tile> tiles = new List<Tile>();
        if (TileManager.Instance != null && TileManager.Instance.freeTiles != null)
        {
            tiles = TileManager.Instance.freeTiles.Where(t => t != null).ToList();
        }
        else
        {
            Debug.LogError("TileManager or its freeTiles list is NULL in SpawnBuilding");
        }

        if (!VerifyResources())
        {
            navButton.gameObject.SetActive(false);
            cantButton.gameObject.SetActive(true);
            Debug.Log("Pas assez de ressources");
            return;
        }

        // Mise � jour visuelle si on change juste de b�timent
        if (previewInstance != null)
        {
            Renderer buildingRenderer = previewInstance.GetComponentInChildren<Renderer>();
            if (buildingRenderer != null)
            {
                buildingRenderer.material = currentBuilding.previewMaterial;
            }
        }

        // Cr�ation d'une nouvelle preview si aucune n'existe
        if (previewInstance == null)
        {
            if (Maincam != null) Maincam.gameObject.SetActive(false);
            if (Secondcam != null) Secondcam.gameObject.SetActive(true);

            // On s'assure que l'index i est valide
            if (tiles.Count > 0)
            {
                // Si l'index i d�passe la nouvelle taille de liste, on le remet � 0
                if (i >= tiles.Count) i = 0;

                if (tiles[i] == null)
                {
                    Debug.LogError("Selected tile is NULL aborting preview spawn");
                    if (navButton != null) navButton.gameObject.SetActive(false);
                    return;
                }

                Vector3 spawnPos = tiles[i].transform.position;
                Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                previewInstance = Instantiate(currentBuilding.prefab, spawnPos, randomRotation);


                Renderer[] allRenderers = previewInstance.GetComponentsInChildren<Renderer>();

                if (allRenderers != null && allRenderers.Length > 0)
                {
                    // Parcourt tous les Renderers trouv�s sur l'objet parent et ses enfants
                    foreach (Renderer renderer in allRenderers)
                    {
                        // Applique le nouveau mat�riau � ce Renderer
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
        List<Tile> tiles = new List<Tile>();
        if (TileManager.Instance != null && TileManager.Instance.freeTiles != null)
            tiles = TileManager.Instance.freeTiles.Where(t => t != null).ToList();
        if (tiles.Count == 0 || previewInstance == null) return;

        // On prend une nouvelle position al�atoire
        i = Random.Range(0, tiles.Count);
        if (tiles[i] == null) return;
        previewInstance.transform.position = tiles[i].transform.position;
    }

    public void PrevPos()
    {
        List<Tile> tiles = new List<Tile>();
        if (TileManager.Instance != null && TileManager.Instance.freeTiles != null)
            tiles = TileManager.Instance.freeTiles.Where(t => t != null).ToList();
        if (tiles.Count == 0 || previewInstance == null) return;

        int newIndex;
        // Petit algo pour essayer d'avoir une position diff�rente de l'actuelle
        int attempts = 0;
        do
        {
            newIndex = Random.Range(0, tiles.Count);
            attempts++;
        } while (tiles.Count > 1 && newIndex == i && attempts < 10);

        i = newIndex;
        if (tiles[i] == null) return;
        previewInstance.transform.position = tiles[i].transform.position;
    }

    private bool VerifyResources()
    {
        if (currentBuilding == null)
        {
            Debug.LogError("VerifyResources called with NULL currentBuilding");
            return false;
        }
        if (RessourcesGestion.RockQuantity >= currentBuilding.rockCost &&
            RessourcesGestion.WoodQuantity >= currentBuilding.woodCost)
        {
            return true;
        }

        return false;
    }

    public void CloseUi()
    {
        if (navButton != null) navButton.gameObject.SetActive(false);
        if (cantButton != null) cantButton.gameObject.SetActive(false);
        if (Maincam != null) Maincam.gameObject.SetActive(true);
        if (Secondcam != null) Secondcam.gameObject.SetActive(false);
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    public void StartBuild()
    {
        if (previewInstance == null) return;

        if (navButton != null) navButton.gameObject.SetActive(false);
        if (cantButton != null) cantButton.gameObject.SetActive(false);
        if (Maincam != null) Maincam.gameObject.SetActive(true);
        if (Secondcam != null) Secondcam.gameObject.SetActive(false);

        List<Tile> tiles = TileManager.Instance.freeTiles;

        // V�rification de s�curit�
        // Defensive: remove null entries and validate TileManager
        if (TileManager.Instance == null || TileManager.Instance.freeTiles == null) return;
        tiles = TileManager.Instance.freeTiles.Where(t => t != null).ToList();
        if (tiles.Count == 0) return;
        if (i >= tiles.Count) i = 0; // S�curit� si l'index est hors limite

        Tile selectedTile = tiles[i];
        if (selectedTile == null) return;
        GameObject buildingToConstruct = previewInstance;
        BuildingData buildingData = currentBuilding;
        TileManager.Instance.RemoveTile(selectedTile);
        previewInstance = null;

        LaunchConstruct(selectedTile, buildingToConstruct, buildingData);
    }

    bool firstConstruct = true;
    public void LaunchConstruct(Tile targetTile, GameObject buildingInstance, BuildingData data)
    {
        if (firstConstruct)
        {
            firstConstruct = false;
			if (NotifManager.Instance != null) NotifManager.Instance.SpawnInfo("drop a builder to build it");
        }
        //if (!VerifyResources())
        //{
        //    navButton.gameObject.SetActive(false);
        //    cantButton.gameObject.SetActive(true);
        //    Debug.Log("Pas assez de ressources");
        //    return;
        //}

        // here, this is better and will not remove resources for nothing
        RessourcesGestion.RemoveRock(currentBuilding.rockCost);
        RessourcesGestion.RemoveWood(currentBuilding.woodCost);

        targetTile.tag = "ToBuild";
        var buildingReference = buildingInstance.AddComponent<BuildingComponent>();
        buildingReference.buildingData = data;
        buildingReference.floorTile = targetTile;
        buildingReference.timeConstructRemain = data.TimeToConstruct;
        buildingReference.isFinish = false;
        if (PlaceManager.Instance != null) PlaceManager.Instance.PlaceDictionary[Place.CONSTRUCTION].Add(buildingInstance);
    }



    bool firstSchool = true;
    bool firstMuseum = true;
    bool firstFarm = true;
    public void FinalizeConstruction(GameObject buildingInstance)
    {
        var buildingReference = buildingInstance.GetComponent<BuildingComponent>();
        buildingReference.isFinish = false;
        Tile targetTile = buildingReference.floorTile;
        BuildingData data = buildingReference.buildingData;
        buildingReference.timeConstructRemain = data.TimeToConstruct;

        targetTile.tag = data.buildingName;
        if (firstSchool && data.buildingName == "School")
        {
            firstSchool = false;
            if (NotifManager.Instance != null) NotifManager.Instance.SpawnInfo("drop a pawn on it to learn a job");
        }
        if (firstMuseum && (data.buildingName == "Library" || data.buildingName == "Museum"))
        {
            firstMuseum = false;
            if (NotifManager.Instance != null) NotifManager.Instance.SpawnInfo("this will add some happyness every day");
        }
        if (firstFarm && data.buildingName == "Farm")
        {
            firstFarm = false;
            if (NotifManager.Instance != null) NotifManager.Instance.SpawnInfo("this will buff food harvest");
        }

        Renderer[] allRenderers = buildingInstance.GetComponentsInChildren<Renderer>();
        if (allRenderers != null && allRenderers.Length > 0)
        {
            // On boucle sur TOUS les Renderers pour appliquer le matriau final
            foreach (Renderer renderer in allRenderers)
            {
                renderer.material = data.finalMaterial;
            }
        }
        // *******************************************************************

        if (NotifManager.Instance != null) NotifManager.Instance.SpawnGoodNews($"a {data.buildingName} is finish");

        if (placeManager != null && placeManager.PlaceDictionary.ContainsKey(data.associatedPlace))
        {
            placeManager.PlaceDictionary[data.associatedPlace].Add(buildingInstance);
            if (placeManager.PlaceDictionary.ContainsKey(Place.CONSTRUCTION))
                placeManager.PlaceDictionary[Place.CONSTRUCTION].Remove(buildingInstance);
        }

    }
}