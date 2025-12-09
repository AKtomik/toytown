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
    public int typeRessources;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /*private void OnMouseDown()
    {
        if(typeRessources == 1)
        {
            Debug.Log("plain");
            Debug.Log("aucune ressource sur les plaines");
        }
        else if (typeRessources == 2)
        {
            RessourcesGestion.AddRock();
            Debug.Log("rock");
        }
        else if (typeRessources == 3)
        {
            RessourcesGestion.AddFood();
            Debug.Log("food");
        }
        else if (typeRessources == 4)
        {
            RessourcesGestion.AddWood();
            Debug.Log("wood");
        }
    }*/


}
