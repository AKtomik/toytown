using UnityEngine;

public class UiBuilding : MonoBehaviour
{
    [SerializeField] private GameObject panelOption;
    [SerializeField] private float duration = 0.5f; // Durée de l'animation en secondes
    [SerializeField] private Vector2 targetPosition;

    private RectTransform rect;
    private Vector2 initialPosition;
    private bool isOpen = false;
    private bool isMoving = false;
    private float elapsed = 0f;

    void Start()
    {
        rect = panelOption.GetComponent<RectTransform>();
        initialPosition = rect.anchoredPosition;
    }

    void Update()
    {
        if (isMoving)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Easing (smoothstep)
            t = t * t * (3f - 2f * t);

            Vector2 start = isOpen ? initialPosition : targetPosition;
            Vector2 end = isOpen ? targetPosition : initialPosition;

            rect.anchoredPosition = Vector2.Lerp(start, end, t);

            if (t >= 1f)
            {
                isMoving = false;
            }
        }
    }

    public void displayOption()
    {
        isOpen = !isOpen;
        isMoving = true;
        elapsed = 0f;
    }
}
