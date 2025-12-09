using TMPro;
using UnityEngine;

public class RessourcesGestion : MonoBehaviour
{
    public static RessourcesGestion Instance;

    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text rockText;
    [SerializeField] private TMP_Text foodText;

    [SerializeField] private int woodQuantity;
    [SerializeField] private int rockQuantity;
    [SerializeField] private int foodQuantity;

    // Accès public en lecture
    public static int WoodQuantity => Instance.woodQuantity;
    public static int RockQuantity => Instance.rockQuantity;
    public static int FoodQuantity => Instance.foodQuantity;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        woodText.text = woodQuantity.ToString();
        rockText.text = rockQuantity.ToString();
        foodText.text = foodQuantity.ToString();
    }

    public static void AddRock()
    {
        Instance.rockQuantity++;
        Instance.rockText.text = Instance.rockQuantity.ToString();
    }

    public static void AddFood()
    {
        Instance.foodQuantity++;
        Instance.foodText.text = Instance.foodQuantity.ToString();
    }

    public static void AddWood()
    {
        Instance.woodQuantity++;
        Instance.woodText.text = Instance.woodQuantity.ToString();
    }

    public static void RemoveRock(int num)
    {
        Instance.rockQuantity -= num;
        Instance.rockText.text = Instance.rockQuantity.ToString();
    }

    public static void RemoveFood(int num)
    {
        Instance.foodQuantity -= num;
        Instance.foodText.text = Instance.foodQuantity.ToString();
    }

    public static void RemoveWood(int num)
    {
        Instance.woodQuantity -= num;
        Instance.woodText.text = Instance.woodQuantity.ToString();
    }
}
