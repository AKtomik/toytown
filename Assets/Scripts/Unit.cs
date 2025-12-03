using UnityEngine;

public enum UnitJob {
    NOTHING,
    FARMER,
    LUMBERJACK,
    MINER,
    BUILDER,
}

public class UnitTask {
    double energyPerDay;
    double saturationPerDay;
    double happynessPerDay;

    private UnitTask(double energyPerDay, double saturationPerDay, double happynessPerDay)
	{
		this.energyPerDay = energyPerDay;
		this.saturationPerDay = saturationPerDay;
		this.happynessPerDay = happynessPerDay;
	}

    public static UnitTask WANDERING = new(-.2, -.2, 0);
    public static UnitTask WORKING = new(-.5, -.5, 0);
    public static UnitTask LEARNING = new(-.5, -.3, 0);
    //public static UnitTaskRepercution WAITING = new(-.2, -.2, 0);
    //public static UnitTaskRepercution WALKING = new(-.2, -.2, 0);
    //public static UnitTaskRepercution EATING = new(-.2, -.2, 0);
}


public class Unit : MonoBehaviour
{
    public double saturationScore = 1;
    public double energyScore = 1;
    public double happynessScore = .5;
    public UnitJob actualJob = UnitJob.NOTHING;
    public UnitJob actualTask = UnitJob.NOTHING;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
