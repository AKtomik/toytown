using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    public List<Tile> freeTiles = new List<Tile>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // On récupère toutes les tuiles à l'initialisation
        GameObject[] plains = GameObject.FindGameObjectsWithTag("Plain");
        foreach (GameObject go in plains)
        {
            Tile tile = go.GetComponent<Tile>();
            freeTiles.Add(tile);
        }
    }

    public void RemoveTile(Tile t)
    {
        freeTiles.Remove(t);
    }

    public void AddTile(Tile tile)
    {
        if (tile != null && !freeTiles.Contains(tile))
            freeTiles.Add(tile);
    }


}
