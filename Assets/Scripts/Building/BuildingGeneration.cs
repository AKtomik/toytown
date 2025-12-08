using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGeneration : MonoBehaviour
{
    [SerializeField] List<Tile> listPos = new List<Tile>();
    [SerializeField] private GameObject SchoolPreview;
    [SerializeField] private Button SchoolButton;
    [SerializeField] private Material SchoolPreviewMat;
    [SerializeField] private Material SchoolMat;


    private int i = 0;
    private GameObject previewInstance;


    public void Awake()
    {
        GameObject[] plains = GameObject.FindGameObjectsWithTag("Plain");

        foreach (GameObject go in plains)
        {
            Tile tile = go.GetComponent<Tile>();
             listPos.Add(tile);
        }
    }
    public void SpawnBuilding()
    {
        if (previewInstance == null)
        {
            Vector3 spawnPos = listPos[i].transform.position + new Vector3(0, 1f, 0);
            previewInstance = Instantiate(SchoolPreview, spawnPos, Quaternion.identity);

            // On applique le matériau de preview
            previewInstance.GetComponent<Renderer>().material = SchoolPreviewMat;

            SchoolButton.gameObject.SetActive(false);
        }
    }


    public void NextPos()
    {
        if (previewInstance == null) return;
        if (i < listPos.Count - 1)
        {
            i++;
            previewInstance.transform.position = listPos[i].transform.position + new Vector3(0, 1f, 0);
        }
    }

    public void PrevPos()
    {
        if (previewInstance == null) return;
        if (i > 0)
        {
            i--;
            previewInstance.transform.position = listPos[i].transform.position + new Vector3(0, 1f, 0);
        }
    }

    public void ValidateSpawn()
    {
        if (previewInstance != null)
        {
            // On remet le matériau original
            previewInstance.GetComponent<Renderer>().material = SchoolMat;

            // On "valide" le bâtiment et on supprime la référence de preview
            previewInstance = null;
            listPos.Remove(listPos[i]);
            SchoolButton.gameObject.SetActive(true);

        }
    }
}
