using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour
{
    double DayAmount = .25;
    float ConstSunYdegree = 30;
    float ConstSunZdegree = 0;
    public double Today
	{
		get
		{
			return DayAmount % 1;
		}
        private set {}
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DayAmount += Time.deltaTime / Settings.DayLengthInSecond;
        float sunX = (float)Today * 360;
        Debug.Log($"1today {Today} sunX {sunX} {transform.rotation.x},{transform.rotation.y},{transform.rotation.z}");
        transform.eulerAngles = new Vector3(sunX, ConstSunYdegree, ConstSunZdegree);
    }
}
