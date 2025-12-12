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

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			//var components = dayCounterObject.GetComponents<Component>();
			//Debug.Log($"Components found: {string.Join(", ", components.Select(c => c.GetType().Name))}");
			if (!dayCounterObject.TryGetComponent(out dayCounterMesh)) Debug.LogError($"no TextMeshPro component in dayCounterObject! {dayCounterObject}");
			if (!BarFillMask.TryGetComponent(out BarFillRectmaskComponent)) Debug.LogError($"no RectMask2D component in BarFillMask! {BarFillMask}");
			if (!BarGrayMask.TryGetComponent(out BarGrayRectmaskComponent)) Debug.LogError($"no RectMask2D component in BarGrayMask! {BarGrayMask}");
		}

		// Update is called once per frame
		void Update()
		{
			// day count
			if (dayCounterMesh != null)
			{
				dayCounterMesh.text = $"Day {Math.Round(SunManager.Instance.DayAmount)}";
			}
			
			// happy count
			float fillProgress = (float)(UnitManager.Instance.ComputeTotalHappyness() / Settings.RequireHappyness);
			float grayProgress = (float)(UnitManager.Instance.UnitCount() / Settings.RequireHappyness);
			BarFillRectmaskComponent.padding = new Vector4(0, 0, BarPixelStartToRight + (BarPixelEndToRight - BarPixelStartToRight) * fillProgress, 0);
			BarGrayRectmaskComponent.padding = new Vector4(0, 0, BarPixelStartToRight + (BarPixelEndToRight - BarPixelStartToRight) * grayProgress, 0);
			// BarPixelStartToRight + (BarPixelEndToRight - BarPixelStartToRight) * grayProgress
		}
	}
}