using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private bool isBuildable;

    /*
    --1 =  plaines
    --2 = rock
    --3 = food
    --4 = wood
    */
    [SerializeField]
    private int typeRessources;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        if(typeRessources == 1)
        {
            Debug.Log("plain");
        }
        else if (typeRessources == 2)
        {
            Debug.Log("rock");
        }
        else if (typeRessources == 3)
        {
            Debug.Log("food");
        }
        else if (typeRessources == 4)
        {
            Debug.Log("wood");
        }
    }


}
