using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGeneration : MonoBehaviour
{
    [SerializeField] private GameObject BuildPreview;
    [SerializeField] private Button BuildButton;
    [SerializeField] private Material BuildPreviewMat;
    [SerializeField] private Material BuildMat;

    [SerializeField] private int RockForFarm;
    [SerializeField] private int WoodForFarm;

    [SerializeField] private int RockForHouse;
    [SerializeField] private int WoodForHouse;

    [SerializeField] private int RockForSchool;
    [SerializeField] private int WoodForSchool;

    [SerializeField] private int RockForMuseum;
    [SerializeField] private int WoodForMuseum;

    [SerializeField] private int RockForLibrary;
    [SerializeField] private int WoodForLibrary;

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
        if (VerifRessources())
        {
            Debug.Log("Bat construit");
            if (previewInstance != null)
            {
                List<Tile> tiles = TileManager.Instance.freeTiles;

                previewInstance.GetComponent<Renderer>().material = BuildMat;

                TileManager.Instance.RemoveTile(tiles[i]);

                previewInstance = null;
                BuildButton.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("pas assez de materiaux");
        }

    }

    public bool VerifRessources()
    {
        if (BuildPreview.tag == "School")
        {
            if(RessourcesGestion.rockQuantity >= RockForSchool && RessourcesGestion.woodQuantity >= WoodForSchool)
            {
                RessourcesGestion.RemoveRock(RockForSchool);
                RessourcesGestion.RemoveWood(WoodForSchool);
                return true;
            }
        }
        else if(BuildPreview.tag == "House")
        {
            if (RessourcesGestion.rockQuantity >= RockForHouse && RessourcesGestion.woodQuantity >= WoodForHouse)
            {
                RessourcesGestion.RemoveRock(RockForHouse);
                RessourcesGestion.RemoveWood(WoodForHouse);
                return true;
            }
        }
        else if (BuildPreview.tag == "Farm")
        {
            if (RessourcesGestion.rockQuantity >= RockForFarm && RessourcesGestion.woodQuantity >= WoodForFarm)
            {
                RessourcesGestion.RemoveRock(RockForFarm);
                RessourcesGestion.RemoveWood(WoodForFarm);
                return true;
            }
        }
        else if (BuildPreview.tag == "Library")
        {
            if (RessourcesGestion.rockQuantity >= RockForLibrary && RessourcesGestion.woodQuantity >= WoodForLibrary)
            {
                RessourcesGestion.RemoveRock(RockForLibrary);
                RessourcesGestion.RemoveWood(WoodForLibrary);
                return true;
            }
        }
        else if (BuildPreview.tag == "Museum")
        {
            if (RessourcesGestion.rockQuantity >= RockForMuseum && RessourcesGestion.woodQuantity >= WoodForMuseum)
            {
                RessourcesGestion.RemoveRock(RockForMuseum);
                RessourcesGestion.RemoveWood(WoodForMuseum);
                return true;
            }
        }
        return false;
    }
}
