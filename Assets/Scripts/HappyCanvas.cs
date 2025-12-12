using TMPro;
using UnityEngine;

namespace ToyTown {
	public class HappyCanvas : MonoBehaviour
	{
		[SerializeField]
		GameObject dayCounterObject;
		TextMesh dayCounterMesh;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			//Debug.Log($"{typeof(dayCounterObject)} {GetComponents(dayCounterObject)}");
			if (!dayCounterObject.TryGetComponent(out dayCounterMesh)) Debug.LogError($"no TextMeshPro component! {dayCounterObject}");;
		}

		// Update is called once per frame
		void Update()
		{
			if (dayCounterMesh != null)
			{
				dayCounterMesh.text = $"Day {SunManager.Instance.DayAmount}";
			}
		}
	}
}