using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace ToyTown {
	public enum Place
	{
		POINT = UnitJob.NOTHING,
		BUSH = UnitJob.FARMER,
		WOOD = UnitJob.LUMBERJACK,
		MINE = UnitJob.MINER,
		CONSTRUCTION = UnitJob.BUILDER,
		CANTINE,
		HOUSE,
		SCHOOL,
	}

	public class PlaceManager : MonoBehaviour
	{
		public static PlaceManager Instance { get; private set; }

		// this is here only for adding places with editor
		// ! do NOT add places here with code
		// use PlaceDictionary instead
		[System.Serializable]
		public struct PlacePositionList
		{
			public Place place;
			public GameObject gameObject;
		}
		public List<PlacePositionList> PlaceEditor = new();
		
		public Dictionary<Place, List<GameObject>> PlaceDictionary = new()
		{
			{Place.BUSH, new()},
			{Place.WOOD, new()},
			{Place.MINE, new()},
			{Place.SCHOOL, new()},
			{Place.CANTINE, new()},
			{Place.HOUSE, new()},
			{Place.CONSTRUCTION, new()},
		};

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			Instance = this;
			foreach (var item in PlaceEditor)
			{
				PlaceDictionary[item.place].Add(item.gameObject);
			}
		}

		// Update is called once per frame
		void Update()
		{
			
		}

		public Vector3 GetNearestPlace(Place place, Vector3 pos)
		{
			if (PlaceDictionary[place].Count == 0)
			{
				throw new Exception($"PlaceDictionary list for {place} is empty but GetNearestPlace is called.");
			}
			Vector3 nearestPos = PlaceDictionary[place][0].transform.position;
			float nearestDistance = Vector3.Distance(PlaceDictionary[place][0].transform.position, pos);
			foreach (GameObject placeObject in PlaceDictionary[place])
			{
				Vector3 placePos = placeObject.transform.position;
				float distance = Vector3.Distance(placePos, pos);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestPos = placePos;
				}
			}
			return nearestPos;
		}

		public bool ExistPlace(Place place, Vector3 pos)
		{
			return PlaceDictionary[place].Count == 0;
		}
	}
}