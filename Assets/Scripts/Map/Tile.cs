using UnityEngine;
using ToyTown;

public class Tile : MonoBehaviour
{
    public bool IsBuilderPresent { get; private set; } = false;

    private int builderCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null && unit.GetActualJob() == UnitJob.BUILDER)
        {
            builderCount++;
            if (builderCount > 0 && !IsBuilderPresent)
            {
                IsBuilderPresent = true;
                Debug.Log($"Tile {name}: BUILDER entré. Construction possible.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null && unit.GetActualJob() == UnitJob.BUILDER)
        {
            builderCount--;

            if (builderCount <= 0 && IsBuilderPresent)
            {
                builderCount = 0;
                IsBuilderPresent = false;
                Debug.Log($"Tile {name}: Dernier BUILDER parti. Construction en pause.");
            }
        }
    }
}