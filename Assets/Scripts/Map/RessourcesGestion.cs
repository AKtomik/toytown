using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RessourcesGestion : MonoBehaviour
{
    public static TextMeshPro woodText;
    public static int woodQuantity;

    public static TextMeshPro rockText;
    public static int rockQuantity;

    public static TextMeshPro foodText;
    public static int foodQuantity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void AddRock()
    {
        rockQuantity += 1;
        Debug.Log(rockQuantity);
        rockText.text += rockQuantity.ToString();       

    }

    public static void AddFood()
    {
        foodQuantity += 1;
        Debug.Log(foodQuantity);
        foodText.text += foodQuantity.ToString();


    }

    public static void AddWood()
    {
        woodQuantity += 1;
        Debug.Log(woodQuantity);
        woodText.text += woodQuantity.ToString();

    }
}
