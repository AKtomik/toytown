using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToyTown
{
    // Assurez-vous que cette énumération correspond à vos valeurs réelles
    // (UnitJob n'est pas inclus ici, mais est supposé exister ailleurs).
    public enum Place
    {
        POINT = 0, // Assumer UnitJob.NOTHING = 0
        BUSH = 1, // Assumer UnitJob.FARMER = 1
        WOOD = 2, // Assumer UnitJob.LUMBERJACK = 2
        MINE = 3, // Assumer UnitJob.MINER = 3
        CONSTRUCTION = 4, // Assumer UnitJob.BUILDER = 4

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

        // --- Configuration Editor (pour l'initialisation depuis l'inspecteur) ---

        [System.Serializable]
        public struct PlacePositionList
        {
            public Place place;
            // Utiliser 'GameObject' sans le '?' pour obliger l'utilisateur à assigner un objet
            // dans l'éditeur (ou laisser l'entrée dans la liste vide).
            public GameObject gameObject;
        }
        public List<PlacePositionList> PlaceEditor = new();

        // --- Dictionnaires Internes ---

        // Initialisation complète du dictionnaire pour garantir que toutes les Places existent
        // et éviter les KeyNotFoundException lors de l'ajout.
        public Dictionary<Place, List<GameObject>> PlaceDictionary = Enum.GetValues(typeof(Place))
            .Cast<Place>()
            .ToDictionary(place => place, place => new List<GameObject>());

        // Dictionnaire pour la correspondance Tag -> Place
        Dictionary<string, Place> GroundTagPlaceDictionary = new()
        {
            {"Plain", Place.POINT},

            {"Bush", Place.BUSH},
            {"Tree", Place.WOOD},
            {"Rock", Place.MINE},
            {"ToBuild", Place.CONSTRUCTION},

            {"NO2", Place.CANTINE},
            {"House", Place.HOUSE},
            {"School", Place.SCHOOL},

            {"Farm", Place.FARM},
            {"NO3", Place.LIBRARY},
            {"NO4", Place.MUSEUM},
        };

        // --- Propriétés Raycast ---

        public float RayGroundRange = 1000f;
        public string GroundLayerName = "Tiles";
        LayerMask RayGroundMask;

        // --- Méthodes MonoBehaviour ---

        void Awake()
        {
            // Initialisation du Singleton
            if (Instance != null && Instance != this)
            {
                // Détruire cet objet pour assurer qu'il n'y ait qu'une seule instance
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initialisation des places définies dans l'éditeur
            foreach (var item in PlaceEditor)
            {
                // ⚠️ Vérification ajoutée pour s'assurer que l'objet n'est pas null
                if (item.gameObject != null)
                {
                    // La clé 'item.place' est garantie d'exister grâce à l'initialisation ci-dessus
                    PlaceDictionary[item.place].Add(item.gameObject);
                }
                else
                {
                    Debug.LogWarning($"[PlaceManager] Le GameObject pour la Place {item.place} est null dans PlaceEditor. Ignoré.");
                }
            }
        }

        void Start()
        {
            // Initialisation du LayerMask
            RayGroundMask = LayerMask.GetMask(GroundLayerName);
        }

        // --- Méthodes Publiques Utilitaires ---

        /// <summary>
        /// Retourne l'objet de lieu le plus proche de la position donnée.
        /// </summary>
        /// <param name="place">Le type de lieu à chercher.</param>
        /// <param name="pos">La position de référence.</param>
        /// <returns>Le GameObject le plus proche, ou null si aucun lieu n'est trouvé.</returns>
        public GameObject GetNearestPlaceObject(Place place, Vector3 pos)
        {
            if (!PlaceDictionary.ContainsKey(place) || PlaceDictionary[place].Count == 0)
            {
                Debug.LogWarning($"PlaceDictionary list for {place} is empty or does not exist. Returning null.");
                return null;
            }

            GameObject nearestObject = null;
            float nearestDistance = float.MaxValue;

            // Utiliser ToList() pour itérer sur une copie (plus sûr si la liste était modifiée ailleurs)
            // Itérer sur la liste spécifique à la Place.
            foreach (GameObject placeObject in PlaceDictionary[place].ToList())
            {
                // 💥 Vérification essentielle : Gère les objets qui ont été détruits (Destroy()) 
                // mais qui sont restés dans la liste.
                if (placeObject == null)
                {
                    // L'objet détruit n'est pas supprimé de la liste ici pour ne pas modifier la collection 
                    // pendant l'itération, mais il est ignoré. Un nettoyage doit être fait ailleurs (UnregisterPlace).
                    continue;
                }

                Vector3 placePos = placeObject.transform.position;
                float distance = Vector3.Distance(placePos, pos);

                if (nearestObject == null || distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = placeObject;
                }
            }

            // Si la boucle s'est exécutée mais tous les objets étaient null, nearestObject reste null.
            return nearestObject;
        }

        /// <summary>
        /// Retourne la position d'un lieu aléatoire du type spécifié.
        /// </summary>
        /// <param name="place">Le type de lieu à chercher.</param>
        /// <returns>La position aléatoire, ou Vector3.zero si aucun lieu valide n'est trouvé.</returns>
        public Vector3 RandomWorkPlace(Place place)
        {
            if (!PlaceDictionary.ContainsKey(place))
            {
                Debug.LogError($"Place {place} is not initialized in PlaceDictionary.");
                return Vector3.zero;
            }

            // Filtrer les objets nuls pour éviter de retourner la position d'un objet détruit
            List<GameObject> validPlaces = PlaceDictionary[place].Where(go => go != null).ToList();

            if (validPlaces.Count == 0)
            {
                Debug.LogWarning($"PlaceDictionary list for {place} is empty or only contains destroyed objects. Returning Vector3.zero.");
                return Vector3.zero;
            }

            return validPlaces[UnityEngine.Random.Range(0, validPlaces.Count)].transform.position;
        }

        /// <summary>
        /// Détermine le type de Place de la tuile (Tile) à la position donnée via Raycast.
        /// </summary>
        /// <param name="pos">La position au-dessus de la tuile.</param>
        /// <returns>Le type de Place de la tuile, ou Place.POINT si non trouvé ou erreur.</returns>
        public Place? GetTilePlace(Vector3 pos)
        {
            Vector3 origin = pos + Vector3.up * 10f;
            Vector3 direction = Vector3.down;

            // La vérification de RayGroundMask.value > 0 garantit que le layer a été trouvé
            if (RayGroundMask.value == 0)
            {
                Debug.LogError($"RayGroundMask value is 0. Layer '{GroundLayerName}' might not exist or is not configured.");
                return Place.POINT;
            }

            if (!Physics.Raycast(origin, direction, out RaycastHit hit, RayGroundRange + 10f, RayGroundMask))
            {
                // Le Raycast n'a rien touché sur le layer spécifié
                return Place.POINT;
            }

            // hit.collider est garanti de ne pas être null si Raycast retourne true
            GameObject groundObject = hit.collider.gameObject;

            // Vérification de sécurité supplémentaire bien que moins probable ici
            if (groundObject == null)
            {
                Debug.LogError($"Raycast hit a collider that belongs to a null GameObject.");
                return Place.POINT;
            }

            string groundTag = groundObject.tag;

            // Utiliser TryGetValue pour éviter une KeyNotFoundException
            if (GroundTagPlaceDictionary.TryGetValue(groundTag, out Place place))
            {
                return place;
            }
            else
            {
                Debug.LogWarning($"Tag [{groundTag}] on object {groundObject.name} does not correspond to any Place.");
                return Place.POINT;
            }
        }

        public bool ExistPlace(Place place, Vector3 pos)
        {
            // Vérifie si la clé existe ET si au moins un objet valide (non null) y est associé.
            return PlaceDictionary.ContainsKey(place) && PlaceDictionary[place].Any(go => go != null);
        }

        /// <summary>
        /// Retourne la position d'un lieu aléatoire de TOUS les types.
        /// </summary>
        /// <returns>Une position aléatoire, ou Vector3.zero si aucune place valide n'est enregistrée.</returns>
        public Vector3 RandomPlace()
        {
            // Concaténer toutes les listes de GameObject, filtrer les objets null, et les mettre dans une liste unique.
            List<GameObject> validPlaces = PlaceDictionary.Values.SelectMany(list => list)
                                                               .Where(go => go != null)
                                                               .ToList();

            if (validPlaces.Count == 0)
            {
                Debug.LogWarning("No valid places are registered in PlaceDictionary.");
                return Vector3.zero;
            }

            return validPlaces[UnityEngine.Random.Range(0, validPlaces.Count)].transform.position;
        }

        public void RegisterPlace(Place place, GameObject placeObject)
        {
            // 💥 Vérification essentielle : S'assurer que l'objet est non-null avant d'ajouter
            if (placeObject == null)
            {
                Debug.LogError($"Attempted to register a null GameObject for place {place}. Registration cancelled.");
                return;
            }

            // Utiliser TryGetValue pour s'assurer que la Place est connue et éviter la KeyNotFoundException
            if (PlaceDictionary.TryGetValue(place, out List<GameObject> list))
            {
                list.Add(placeObject);
                Debug.Log($"[PlaceManager] Enregistré : {placeObject.name} comme {place}. Total: {list.Count}");
            }
            else
            {
                Debug.LogError($"Attempted to register place {placeObject.name} for an unknown Place enum value: {place}.");
            }
        }

        public void UnregisterPlace(Place place, GameObject placeObject)
        {
            if (PlaceDictionary.TryGetValue(place, out List<GameObject> list))
            {
                bool removed = list.Remove(placeObject);
                if (!removed)
                {
                    Debug.LogWarning($"[PlaceManager] Failed to unregister {placeObject?.name ?? "null object"} from {place}.");
                }
            }
        }
    }
}