using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ToyTown {
	public class HappyCanvas : MonoBehaviour
	{
		[SerializeField]
		GameObject dayCounterObject;
		TextMeshProUGUI dayCounterMesh;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			//var components = dayCounterObject.GetComponents<Component>();
			//Debug.Log($"Components found: {string.Join(", ", components.Select(c => c.GetType().Name))}");
			if (!dayCounterObject.TryGetComponent(out dayCounterMesh)) Debug.LogError($"no TextMeshPro component! {dayCounterObject}");;
		}

		// Update is called once per frame
		void Update()
		{
			if (dayCounterMesh != null)
			{
				dayCounterMesh.text = $"Day {Math.Round(SunManager.Instance.DayAmount)}";
			}
		}
	}
}