using UnityEngine;
using ToyTown;
using System.Linq;
using System.Collections.Generic;

namespace ToyTown
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance { get; private set; }

        private PlaceManager placeManager;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            placeManager = PlaceManager.Instance;
            if (placeManager == null)
            {
                Debug.LogError("[BuildManager] PlaceManager n'est pas initialisé ou introuvable. Le comptage des bâtiments ne fonctionnera pas.");
            }
        }

        public int GetBuildAmount(Place place)
        {
            if (placeManager == null) return 0;

            if (placeManager.PlaceDictionary.TryGetValue(place, out List<GameObject> placeList))
            {
                return placeList.Count(go => go != null);
            }

            return 0;
        }

        // Exemple : BuildManager.Instance.GetBuildAmount(Place.LIBRARY)

    }
}