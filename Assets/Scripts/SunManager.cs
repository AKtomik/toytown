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
		public double DayAmount;
		public float ConstSunYdegree = 30;
		public float ConstSunZdegree = 0;
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
			Debug.Log($"mono sunManager started");
			Instance = this;
			DayAmount = StartingDayTime;
		}

		// Update is called once per frame
		void Update()
		{
			DayAmount += Time.deltaTime * Settings.SpeedUp / Settings.DayLengthInSecond;
			float sunX = (float)Today * 360;
			//Debug.Log($"1today {Today} sunX {sunX} {transform.rotation.x},{transform.rotation.y},{transform.rotation.z}");
			directionalLight.transform.eulerAngles = new Vector3(sunX, ConstSunYdegree, ConstSunZdegree);
		}
	}
}