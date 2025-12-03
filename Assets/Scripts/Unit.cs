using UnityEngine;

public enum UnitJob {
    NOTHING,
    FARMER,
    LUMBERJACK,
    MINER,
    BUILDER,
}


public class Unit : MonoBehaviour
{
    public double saturationScore = 1;
    public double energyScore = 1;
    public double happynessScore = .5;
    public UnitJob actualJob = UnitJob.NOTHING;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
