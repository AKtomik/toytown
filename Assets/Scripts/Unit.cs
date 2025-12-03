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
		double factor = delta / Settings.DayLengthInSecond;
		unit.energyScore += this.energyPerDay * factor;
		unit.saturationScore += this.saturationPerDay * factor;
		unit.happynessScore += this.happynessPerDay * factor;
	}

    public static UnitTask WANDERING = new(-.2, -.2, 0);
    public static UnitTask WORKING = new(-.5, -.5, .1);
    public static UnitTask LEARNING = new(-.5, -.3, 0);
    //public static UnitTask WAITING = new(-.2, -.2, 0);
    //public static UnitTask WALKING = new(-.2, -.2, 0);

    // temporary
    public static UnitTask EATING = new(-.1, 10, 0);
    public static UnitTask SLEEPING = new(10, -.1, 0);
}


public class Unit : MonoBehaviour
{
    public double saturationScore = 1;
    public double energyScore = 1;
    public double happynessScore = .5;
    public UnitTask actualTask = UnitTask.WANDERING;
    public UnitJob actualJob = UnitJob.NOTHING;
        
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
