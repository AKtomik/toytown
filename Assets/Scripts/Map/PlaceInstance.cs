using UnityEngine;

namespace ToyTown
{
    public class PlaceInstance : MonoBehaviour
    {
        [Tooltip("Quel type de lieu est cet objet ? Doit correspondre à l'enum Place.")]
        public Place placeType;

        void Start()
        {
            if (PlaceManager.Instance != null)
            {
                PlaceManager.Instance.RegisterPlace(placeType, gameObject);
            }
        }
    }
}