using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ToyTown {
	public class HappyCanvas : MonoBehaviour
	{
		[SerializeField]
		GameObject dayCounterObject;
		TextMeshProUGUI dayCounterMesh;
		
		[SerializeField]
		GameObject BarFillMask;
		RectMask2D BarFillRectmaskComponent;
		[SerializeField]
		GameObject BarGrayMask;
		RectMask2D BarGrayRectmaskComponent;
		[SerializeField]
		int BarPixelStartToRight;
		[SerializeField]
		int BarPixelEndToRight;
		
		[SerializeField]
		GameObject unitCounterObject;
		[SerializeField]
		TextMeshProUGUI unitCounterMesh;
		
		[SerializeField]
		public struct JobTextUi
		{
			public UnitJob job;
			public GameObject textObject;
		}
		public struct JobTextMesh
		{
			public TextMeshProUGUI mesh;
			public GameObject textObject;
		}
		JobTextUi unitJobCounterObject;
		JobTextMesh unitJobCounterMesh;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			//var components = dayCounterObject.GetComponents<Component>();
			//Debug.Log($"Components found: {string.Join(", ", components.Select(c => c.GetType().Name))}");
			if (!dayCounterObject.TryGetComponent(out dayCounterMesh)) Debug.LogError($"no TextMeshProFEZ component in dayCounterObject! {dayCounterObject}");
			if (!unitCounterObject.TryGetComponent(out unitCounterMesh)) Debug.LogError($"no TextMeshProIBF component in unitCounterObject! {unitCounterObject}");
			if (!BarFillMask.TryGetComponent(out BarFillRectmaskComponent)) Debug.LogError($"no RectMask2D component in BarFillMask! {BarFillMask}");
			if (!BarGrayMask.TryGetComponent(out BarGrayRectmaskComponent)) Debug.LogError($"no RectMask2D component in BarGrayMask! {BarGrayMask}");
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
			
		}
	}
}