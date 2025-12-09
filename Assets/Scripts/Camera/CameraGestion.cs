using UnityEngine;

public class CameraGestion : MonoBehaviour
{
    public float scrollSpeed;

    [Header("Barrières d'écran (détection souris)")]
    [SerializeField] private float topBarrier = 0.9f;
    [SerializeField] private float botBarrier = 0.1f;
    [SerializeField] private float leftBarrier = 0.1f;
    [SerializeField] private float rightBarrier = 0.9f;

    [Header("Limites de déplacement")]
    public float minX = -20f;
    public float maxX = 20f;
    public float minZ = -20f;
    public float maxZ = 20f;

    void Update()
    {
        Vector3 move = Vector3.zero;

        if (Input.mousePosition.y >= Screen.height * topBarrier)
            move += Vector3.forward;

        if (Input.mousePosition.y <= Screen.height * botBarrier)
            move += Vector3.back;

        if (Input.mousePosition.x >= Screen.width * rightBarrier)
            move += Vector3.right;

        if (Input.mousePosition.x <= Screen.width * leftBarrier)
            move += Vector3.left;

        // Applique le mouvement
        transform.Translate(move * scrollSpeed * Time.deltaTime, Space.World);

        // Clamp la position pour empêcher d'aller trop loin
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.z = Mathf.Clamp(clampedPos.z, minZ, maxZ);
        transform.position = clampedPos;
    }
}
