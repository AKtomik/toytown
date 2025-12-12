using UnityEngine;

namespace ToyTown
{
    public class PlaceInstance : MonoBehaviour
    {
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