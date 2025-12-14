using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


namespace ToyTown
{
	public class SunManager : MonoBehaviour
	{
		public static SunManager Instance { get; private set; }

		public Light directionalLight;

		public double StartingDayTime = .25;
		double DayTime;
		public float ConstSunYdegree = 30;
		public float ConstSunZdegree = 0;
		public double Today
		{
			get
			{
				return DayTime % 1;
			}
		}

		public int DayAmount
		{
			get
			{
				return (int)Math.Floor(DayTime);
			}
		}

		public bool IsDay
		{
			get
			{
				return Today < .5;
			}
			private set {}
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			Debug.Log($"mono sunManager started");
			Instance = this;
			DayTime = StartingDayTime;
		}

		// Update is called once per frame
		void Update()
		{
			DayTime += Time.deltaTime * Settings.SpeedUp / Settings.DayLengthInSecond;
			float sunX = (float)Today * 360;
			//Debug.Log($"1today {Today} sunX {sunX} {transform.rotation.x},{transform.rotation.y},{transform.rotation.z}");
			directionalLight.transform.eulerAngles = new Vector3(sunX, ConstSunYdegree, ConstSunZdegree);
		}
	}
}