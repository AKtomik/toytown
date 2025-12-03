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

    public void UpdateRepercution(Unit unit, double delta)
	{
		double factor = delta / 60;
		unit.energyScore += this.energyPerDay * factor;
		unit.saturationScore += this.saturationPerDay * factor;
		unit.happynessScore += this.happynessPerDay * factor;
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
    public UnitTask actualTask = UnitTask.WANDERING;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.actualTask.UpdateRepercution(this, Time.deltaTime);
    }
}
