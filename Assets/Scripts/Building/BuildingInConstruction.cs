using System.Collections;
using UnityEngine;

public class BuildingInConstruction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BuildInProgress ()
    {
        yield return new WaitForSeconds(5);
        print("WaitAndPrint " + Time.time);
    }
}
