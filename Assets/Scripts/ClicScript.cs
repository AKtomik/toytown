using UnityEngine;
using UnityEngine.EventSystems;

public class SceneClickHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return; // Ignore le clic si on clique sur un UI

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Clic sur la scène : " + hit.collider.name);
            }
        }
    }
}
