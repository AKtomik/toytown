using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum Place
	{
		BUSH,
        WOOD,
        MINE,
        SCHOOL,
        CANTINE,
        HOUSE,
	}
    
    public Dictionary<Place, List<Vector3>> PlacePlacing = new()
	{
		{Place.BUSH, new()},
        {Place.WOOD, new()},
        {Place.MINE, new()},
        {Place.SCHOOL, new()},
        {Place.CANTINE, new()},
        {Place.HOUSE, new()},
	};

    public Vector3 GetNearestPlace(Place place, Vector3 pos)
	{
        if (PlacePlacing[place].Count == 0)
		{
			throw new Exception($"PlacePlacing list for {place} is empty but GetNearestPlace is called.");
		}
        Vector3 nearestPos = PlacePlacing[place][0];
        float nearestDistance = Vector3.Distance(PlacePlacing[place][0], pos);
        foreach (Vector3 placePos in PlacePlacing[place])
		{
			float distance = Vector3.Distance(placePos, pos);
            if (distance < nearestDistance)
			{
				nearestDistance = distance;
                nearestPos = placePos;
			}
		}
		return nearestPos;
	}
}
