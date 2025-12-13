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

		[SerializeField]
		GameObject oneFullSection;
		[SerializeField]
		TextMeshProUGUI oneText;
		[SerializeField]
		GameObject oneJobsSection;
		

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

			// show unit
			RefreshUnitSelect();
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
		string Capitalize(string input)
		{
			return char.ToUpper(input[0]) + input[1..].ToLower();
		}
		void RefreshUnitSelect()
		{
			Unit unit = GrabManager.Instance.lastGrabed;
			if (unit == null)
			{
				oneFullSection.SetActive(false);
				return;
			}
			oneFullSection.SetActive(true);
			
			if (unit.isAdult)
			{
				string actionText = unit.GetActualAction().ToString().ToLower();
				if (unit.IsWalking())
				{
					actionText = $"going to {actionText[..(actionText.Length - 3)]}";
				}
				oneText.text = $"is a {Capitalize(unit.GetActualJob().ToString())} and is {actionText}";
			}
			else
			{
				oneText.text = $"is a child";
			}
			int dayAge = (int)Math.Floor(unit.age);
			if (dayAge == 0)
				oneText.text += $"\nis born today";
			else if (dayAge == 1)
				oneText.text += $"\nis born since {dayAge} day";
			else
				oneText.text += $"\nis born since {dayAge} days";

			
			if (!unit.IsWaitingLearningJob())
			{
				oneJobsSection.SetActive(false);
				return;
			}
			oneJobsSection.SetActive(true);
		}
	}
}