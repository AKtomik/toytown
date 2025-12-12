using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ToyTown {
	public class HappyCanvas : MonoBehaviour
	{
		[SerializeField]
		TextMeshProUGUI dayCounterMesh;
		
		[SerializeField]
		RectMask2D BarFillRectmaskComponent;
		[SerializeField]
		RectMask2D BarGrayRectmaskComponent;
		[SerializeField]
		int BarPixelStartToRight;
		[SerializeField]
		int BarPixelEndToRight;
		
		[SerializeField]
		TextMeshProUGUI unitCounterMesh;
		
		[System.Serializable]
		public struct JobTextMesh
		{
			public UnitJob job;
			public TextMeshProUGUI mesh;
		}
		[SerializeField]
		List<JobTextMesh> unitJobCounterMesh;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			//var components = dayCounterObject.GetComponents<Component>();
			//Debug.Log($"Components found: {string.Join(", ", components.Select(c => c.GetType().Name))}");
		}

		// Update is called once per frame
		void Update()
		{
			// day count
			if (dayCounterMesh != null) dayCounterMesh.text = $"{(SunManager.Instance.IsDay ? "Day" : "Night")} {Math.Round(SunManager.Instance.DayAmount)}";
			
			// happy count
			RefreshHappyness();

			// unit count
			RefreshUnitCount();
		}

		void RefreshHappyness()
		{
			float fillProgress = (float)(UnitManager.Instance.ComputeTotalHappyness() / Settings.RequireHappyness);
			float grayProgress = (float)(UnitManager.Instance.UnitCount() / Settings.RequireHappyness);
			BarFillRectmaskComponent.padding = new Vector4(0, 0, BarPixelStartToRight + (BarPixelEndToRight - BarPixelStartToRight) * fillProgress, 0);
			BarGrayRectmaskComponent.padding = new Vector4(0, 0, BarPixelStartToRight + (BarPixelEndToRight - BarPixelStartToRight) * grayProgress, 0);
		}

		void RefreshUnitCount()
		{
			if (unitCounterMesh != null) unitCounterMesh.text = UnitManager.Instance.UnitCount().ToString();
			var jobCount = UnitManager.Instance.UnitCountByJobs();
			foreach (var jobAndMeshStruct in unitJobCounterMesh)
			{
				jobAndMeshStruct.mesh.text = jobCount[jobAndMeshStruct.job].ToString();
			}
		}
	}
}