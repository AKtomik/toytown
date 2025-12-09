using TMPro;
using UnityEngine;

public class RessourcesGestion : MonoBehaviour
{
    [SerializeField] private TMP_Text woodText;
    public static int woodQuantity;

    [SerializeField] private TMP_Text rockText;
    public static int rockQuantity;

    [SerializeField] private TMP_Text foodText;
    public static int foodQuantity;

    private static RessourcesGestion instance;

    private void Awake()
    {
        instance = this;
        foodText.text = "0";
        woodText.text = "0";
        rockText.text = "0";
    }

    public static void AddRock()
    {
        rockQuantity++;
        instance.rockText.text = rockQuantity.ToString();
    }

    public static void AddFood()
    {
        foodQuantity++;
        instance.foodText.text = foodQuantity.ToString();
    }

    public static void AddWood()
    {
        woodQuantity++;
        instance.woodText.text = woodQuantity.ToString();
    }

    public static void RemoveRock(int num)
    {
        rockQuantity -= num;
        instance.rockText.text = rockQuantity.ToString();
    }

    public static void RemoveFood(int num)
    {
        foodQuantity -= num;
        instance.foodText.text = foodQuantity.ToString();
    }

    public static void RemoveWood(int num)
    {
        woodQuantity -= num;
        instance.woodText.text = woodQuantity.ToString();
    }
}
