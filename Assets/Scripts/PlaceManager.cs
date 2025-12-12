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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            foreach (var item in PlaceEditor)
            {
                if (PlaceDictionary.ContainsKey(item.place)) 
                {
                    PlaceDictionary[item.place].Add(item.gameObject);
                }
                else
                {
                    Debug.LogError($"[PlaceManager.Awake] La clé '{item.place}' (Valeur: {(int)item.place}) n'existe pas dans PlaceDictionary. Veuillez mettre à jour PlaceDictionary.");
                }
            }
        }

        // Update is called once per frame
        void Update()
		{

		}

        public Vector3 GetNearestPlace(Place place, Vector3 pos)
        {
            List<GameObject> places = PlaceDictionary[place];

            // 1. Gérer la liste vide (cette partie est déjà là, mais on la garde)
            if (places.Count == 0)
            {
                throw new Exception($"PlaceDictionary list for {place} is empty but GetNearestPlace is called.");
            }

            // 2. Trouver un point de départ non-null
            GameObject startObject = places.Find(obj => obj != null);

            if (startObject == null)
            {
                // La liste contient des éléments, mais ils sont tous nuls. Nettoyons et retournons une erreur.
                Debug.LogError($"Toutes les entrées pour {place} dans PlaceDictionary sont nulles! Nettoyage nécessaire.");
                places.RemoveAll(obj => obj == null);
                throw new Exception($"Tous les GameObjects pour {place} sont détruits.");
            }

            Vector3 nearestPos = startObject.transform.position;
            float nearestDistance = Vector3.Distance(nearestPos, pos);

            // 3. Itérer et vérifier
            foreach (GameObject placeObject in places)
            {
                if (placeObject == null) continue; // <-- IGNORER LES OBJETS DÉTRUITS

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

        public float RayGroundRange = 100f;
		public int? RayGroundMask = 2^7;
		
		public Place? GetTilePlace(Vector3 pos)
		{
			Vector3 origin = pos + Vector3.up * .5f;
			Vector3 direction = Vector3.down;

			RaycastHit hit;
			if (RayGroundMask.HasValue)
				Physics.Raycast(origin, direction, out hit, RayGroundRange, RayGroundMask.Value);
			else
				Physics.Raycast(origin, direction, out hit, RayGroundRange);
			GameObject gameObject = hit.collider.gameObject;
			Debug.Log($"FALL ON gameObject {gameObject}");
			return Place.BUSH;
		}

		public bool ExistPlace(Place place, Vector3 pos)
		{
            return PlaceDictionary.ContainsKey(place) && PlaceDictionary[place].Count > 0;
        }

        public void RegisterPlace(Place place, GameObject placeObject)
        {
            PlaceDictionary[place].Add(placeObject);
            Debug.Log($"[PlaceManager] Enregistré : {placeObject.name} comme {place}. Total: {PlaceDictionary[place].Count}");
        }

        public void UnregisterPlace(Place place, GameObject placeObject)
        {
			bool removed = PlaceDictionary[place].Remove(placeObject);
        }
    }
}