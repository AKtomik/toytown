using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToyTown {
	public class UnitManager : MonoBehaviour
	{
		public static UnitManager Instance { get; private set; }
		[SerializeField]
		private GameObject unitPrefab;
		private double spawnProgress = 1;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			Instance = this;
			Debug.Log($"mono unitManager started");
			if (unitPrefab == null) throw new Exception($"unitPrefab is not defined, assign it in the unity editor in UnitManager!");
			Instantiate(unitPrefab, transform.position, transform.rotation);
		}

		// Update is called once per frame
		void Update()
		{
			double rand = Random.value * 2 - 1;
			double variation = 1 + rand * Random.value;
			if (variation < 0) variation = 1 / Math.Abs(variation);
			spawnProgress += Time.deltaTime * Settings.SpeedUp / Settings.DayLengthInSecond * variation;
			if (spawnProgress > 1)
			{
				spawnProgress -= 1;
				Debug.Log($"spawning a new unit at UnitManager");
				if (unitPrefab == null) throw new Exception($"unitPrefab is not defined, assign it in the unity editor in UnitManager!");
				Instantiate(unitPrefab, transform.position, transform.rotation);
			}
		}

		public Unit[] UnitArray()
		{
			return FindObjectsByType<Unit>(FindObjectsSortMode.None);
		}

		public int UnitCount()
		{
			return UnitArray().Length;
		}
		
		public Dictionary<UnitJob, int> UnitCountByJobs()
		{
			Dictionary<UnitJob, int> jobCount = new();
			foreach (UnitJob job in Enum.GetValues(typeof(UnitJob)).Cast<UnitJob>())
			{
				jobCount[job] = 0;
			}
			foreach (Unit unit in UnitArray())
			{
				jobCount[unit.GetActualJob()] += 1;
			}
			return jobCount;
		}

		public double ComputeTotalHappyness()
		{
			Unit[] units = UnitArray();
			double happnessProgress = 0;
			foreach (Unit unit in units)
			{
				happnessProgress += unit.happynessScore;
			}
			return happnessProgress;
		}
	}
}