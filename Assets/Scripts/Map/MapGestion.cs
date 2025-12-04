using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MapGestion : MonoBehaviour
{
    [SerializeField]
    List<Tile> ListForest = new List<Tile>();

    [SerializeField]
    List<Tile> ListRock = new List<Tile>();

    [SerializeField]
    List<Tile> ListBush = new List<Tile>();

    [SerializeField]
    List<Tile> ListPlaines = new List<Tile>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
