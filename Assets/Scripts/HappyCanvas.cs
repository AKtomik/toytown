using TMPro;
using UnityEngine;

namespace ToyTown {
	public class HappyCanvas : MonoBehaviour
		{
			[SerializeField]
			TextMeshPro dayCounter;

			// Start is called once before the first execution of Update after the MonoBehaviour is created
			void Start()
			{
				
			}

			// Update is called once per frame
			void Update()
			{
				dayCounter.text = $"Day {SunManager.Instance.DayAmount}";
			}
		}
}