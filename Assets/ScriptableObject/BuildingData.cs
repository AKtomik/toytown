using ToyTown;
using UnityEngine;

[CreateAssetMenu(menuName = "Building/BuildingData")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public GameObject prefab;
    public int woodCost;
    public int rockCost;
    public Material previewMaterial;
    public Material finalMaterial;
    public Place associatedPlace;
    public float TimeToConstruct;
    public bool isFull;
}
