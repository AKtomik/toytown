using UnityEngine;

namespace ToyTown {
	public class UnitManager : MonoBehaviour
	{
		public static UnitManager Instance { get; private set; }

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			Instance = this;
		}

		// Update is called once per frame
		void Update()
		{
			
		}

		public Unit[] UnitArray()
		{
			return FindObjectsByType<Unit>(FindObjectsSortMode.None);
		}

		public int UnitCount()
		{
			return UnitArray().Length;
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