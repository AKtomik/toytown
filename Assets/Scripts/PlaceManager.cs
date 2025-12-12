using System;
using System.Collections.Generic;
using System.Linq;
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
		
		CANTINE = 11,
		HOUSE = 12,
		SCHOOL = 13,

		FARM = 21,
		LIBRARY = 22,
		MUSEUM = 23,
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
			{Place.CONSTRUCTION, new()},

			{Place.CANTINE, new()},
			{Place.HOUSE, new()},
			{Place.SCHOOL, new()},
			
			{Place.FARM, new()},
			{Place.LIBRARY, new()},
			{Place.MUSEUM, new()},
		};
		
		Dictionary<string, Place> GroundTagPlaceDictionary = new()
		{
			{"Plain", Place.POINT},

			{"Bush", Place.BUSH},
			{"Tree", Place.WOOD},
			{"Rock", Place.MINE},
			{"ToBuild", Place.CONSTRUCTION},

			{"NO2", Place.CANTINE},
			{"House", Place.HOUSE},
			{"Scool", Place.SCHOOL},
			
			{"Farm", Place.FARM},
			{"NO3", Place.LIBRARY},
			{"NO4", Place.MUSEUM},
		};

		void Awake()
		{
			Debug.Log($"mono placeManager awaked");
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;

			// Vous pouvez laisser l'ajout PlaceEditor dans Awake ou Start, 
			// mais si des PlaceInstance en Start ont besoin des données Editor, laissez-le ici.
			foreach (var item in PlaceEditor)
			{
				PlaceDictionary[item.place].Add(item.gameObject);
			}
		}

		public float RayGroundRange = 1000f;
		public string GroundLayerName = "Tiles";
		LayerMask RayGroundMask;
		
		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			Debug.Log($"mono placeManager started");
			RayGroundMask = LayerMask.GetMask(GroundLayerName);
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

		public Place? GetTilePlace(Vector3 pos)
		{
			Vector3 origin = pos + Vector3.up * 10f;
			Vector3 direction = Vector3.down;

			Physics.Raycast(origin, direction, out RaycastHit hit, RayGroundRange + 10f, RayGroundMask);
			if (hit.collider == null) {
				Debug.LogError($"there is no collided ground (mask {RayGroundMask.value}={GroundLayerName})");
				return Place.POINT;
			}
			GameObject groundObject = hit.collider.gameObject;
			if (groundObject == null) {
				Debug.LogError($"there is no game ground (mask {RayGroundMask}={GroundLayerName})");
				return Place.POINT;
			}
			Debug.Log($"FALL ON tag [{groundObject.tag}]");
			string groundTag = groundObject.tag;
			if (!GroundTagPlaceDictionary.Keys.Contains(groundTag)) {
				Debug.LogError($"there is no place corresponding to the tag [{groundTag}] (object {groundObject})");
				return Place.POINT;
			}
			return GroundTagPlaceDictionary[groundTag];
		}

		public bool ExistPlace(Place place, Vector3 pos)
		{
			return PlaceDictionary.ContainsKey(place) && PlaceDictionary[place].Count > 0;
		}

		public Vector3 RandomPlace()
		{
			List<GameObject> placeList = new();
			foreach (Place place in PlaceDictionary.Keys)
				foreach (GameObject placeObject in PlaceDictionary[place])
					placeList.Add(placeObject);
			return placeList[UnityEngine.Random.Range(0, placeList.Count)].transform.position;
		}

		public void RegisterPlace(Place place, GameObject placeObject)
		{
			PlaceDictionary[place].Add(placeObject);
			Debug.Log($"[PlaceManager] Enregistr� : {placeObject.name} comme {place}. Total: {PlaceDictionary[place].Count}");
		}

		public void UnregisterPlace(Place place, GameObject placeObject)
		{
			bool removed = PlaceDictionary[place].Remove(placeObject);
		}
	}
}